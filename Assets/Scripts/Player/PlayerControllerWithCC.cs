using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Unity.VisualScripting;
using System.Net.Http;
using Inventory;
using Photon.Realtime;
using UnityEngine.SceneManagement;

namespace Objects.PlayerScripts
{
    public class PlayerControllerWithCC : MonoBehaviourPunCallbacks
    {
        [Header("Move settings")]
        [SerializeField] private float moveSpeed = 4.0f;
        [SerializeField] private float jumpForce = 1.0f;
        [SerializeField] private float gravity = -9.81f;
        [SerializeField] private Transform groundCheck;
        [SerializeField] private float groundDistance = 0.4f;
        [SerializeField] private LayerMask groundMask;
        [SerializeField] private Camera playerCamera;
        [SerializeField] private float cameraSensitivity = 2f;
        [SerializeField] private float slopeForce = 5.0f;
        [SerializeField] private float slopeForceRayLength = 1.5f;
        
        public bool isCanRotate = true;
        
        [Header("Footstep Sounds")]
        [SerializeField] private AudioClip[] dirtClips;
        [SerializeField] private AudioClip[] concreteClips;
        [SerializeField] private AudioClip[] metalClips;
        [SerializeField] private AudioClip[] defaultClips;
        [SerializeField] private float stepInterval = 0.5f; // Interval between steps

        [Header("MVC")]
        [SerializeField] private CharacterView view;

        [Header("Params")]
        [SerializeField] private int maxHealth = 100;
        [SerializeField] private float healthRegenInterval = 2f;
        [SerializeField] private int healthaRegenAmount = 1;

        [SerializeField] private int maxMana = 100;
        [SerializeField] private int manaRegenAmount = 1;      
        [SerializeField] private float manaRegenInterval = 0.5f;
        private float curManaRegenInterval;
        private float timer;

        [Header("UI")]
        [SerializeField] private GameObject canvas;

        private CharacterController controller;
        private Vector3 velocity;
        private bool isGrounded;
        private float stepTimer = 0.0f;
        private AudioSource audioSource;

        private Dictionary<PhysicMaterial, AudioClip[]> materialSounds;

        private float _rotationX;
        private PhotonView _photonView;
        [SerializeField] private CharacterModel model;                
        private void Start()
        {
            _photonView = GetComponent<PhotonView>();

            controller = GetComponent<CharacterController>();

            audioSource = GetComponent<AudioSource>();

            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            if (!_photonView.IsMine)
            {
                Destroy(GetComponentInChildren<Camera>().gameObject);
                Destroy(controller);
                Destroy(canvas);
            }
            else
            {
                DurabilityDefenseDatabase durabilDatabase = GameObject.FindWithTag("DurabilBase").GetComponent<DurabilityDefenseDatabase>();               
                durabilDatabase.itemDatabase = gameObject.GetComponent<ItemDatabase>();                             
                model = gameObject.AddComponent<CharacterModel>();
                model.Initialize(maxHealth, maxMana, view, moveSpeed, this, jumpForce, durabilDatabase);

                view.UpdateHealthText(model.Health);
                view.UpdateManaText(model.Mana);
                view.UpdateDefenseText(model.Defense);
                view.UpdateSpeedText(model.Speed);
                view.UpdateJumpForceText(model.JumpForce);

                curManaRegenInterval = manaRegenInterval;
                StartCoroutine(RegenerateMana());
                StartCoroutine(RegenerateHealth());

                if (PhotonNetwork.IsMasterClient) 
                {
                    GameObject.FindWithTag("Chest").GetComponent<Chest>().
                        GenerateItems(gameObject.GetComponent<ItemDatabase>().allItems, durabilDatabase);
                }               
            }

            materialSounds = new Dictionary<PhysicMaterial, AudioClip[]>
            {
                { Resources.Load<PhysicMaterial>("Materials/PhysicalMaterials/Dirt"), dirtClips },
                { Resources.Load<PhysicMaterial>("Materials/PhysicalMaterials/Concrete"), concreteClips },
                { Resources.Load<PhysicMaterial>("Materials/PhysicalMaterials/Metal"), metalClips }
            };
        }

        private void Update()
        {
            if (!_photonView.IsMine) return;

            if (isCanRotate)
            {
                RotatePlayerRightLeft();
                RotateCameraUpDown();
            }           

            isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

            if (Input.GetButtonDown("Jump") && isGrounded)
            {
                TryJump();
            }
        }

        public void UpdateSpeed(float speed)
        {
            moveSpeed = speed;
        }

        public void UpdateJumpForce(float force)
        {
            jumpForce = force;
        }

        private void TryJump()
        {
            velocity.y = Mathf.Sqrt(jumpForce * -2f * gravity);
        }

        private void FixedUpdate()
        {
            if (!_photonView.IsMine) return;

            PlayerMovement();
        }

        private void PlayerMovement()
        {
            float horizontalInput = Input.GetAxis("Horizontal");
            float verticalInput = Input.GetAxis("Vertical");

            Vector3 moveDirection = transform.forward * verticalInput + transform.right * horizontalInput;
            controller.Move(moveDirection * moveSpeed * Time.deltaTime);

            if (isGrounded && new Vector3(controller.velocity.x, 0, controller.velocity.z).magnitude > 0.1f)
            {
                stepTimer += Time.deltaTime;
                if (stepTimer >= stepInterval)
                {
                    photonView.RPC("PlayFootstepSound", RpcTarget.All);
                    stepTimer = 0f;
                }
            }

            if (isGrounded && velocity.y < 0)
            {
                velocity.y = -2f;
            }

            if ((horizontalInput != 0 || verticalInput != 0) && OnSlope())
            {
                moveDirection += Vector3.down * slopeForce;
            }

            velocity.y += gravity * Time.deltaTime;
            controller.Move(velocity * Time.deltaTime);
        }

        private bool OnSlope()
        {
            if (isGrounded)
            {
                RaycastHit hit;
                if (Physics.Raycast(transform.position, Vector3.down, out hit, controller.height / 2 * slopeForceRayLength))
                {
                    if (hit.normal != Vector3.up)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        private void RotatePlayerRightLeft()
        {
            transform.Rotate(Vector3.up, Input.GetAxisRaw("Mouse X") * cameraSensitivity);
        }

        private void RotateCameraUpDown()
        {
            _rotationX -= cameraSensitivity * Input.GetAxisRaw("Mouse Y");
            _rotationX = Mathf.Clamp(_rotationX, -75, 75);
            playerCamera.transform.eulerAngles = new Vector3(_rotationX,
                playerCamera.transform.eulerAngles.y, playerCamera.transform.eulerAngles.z);
        }

        [PunRPC]
        private void PlayFootstepSound()
        {
            RaycastHit hit;
            if (Physics.Raycast(transform.position, Vector3.down, out hit, 2.5f))
            {
                PhysicMaterial material = hit.collider.sharedMaterial;

                if (audioSource == null)
                {
                    Debug.LogError("AudioSource не знайдений.");
                    return;
                }

                if (audioSource.isPlaying)
                {
                    Debug.LogWarning("Аудіо вже відтворюється.");
                    return;
                }

                if (hit.collider == null)
                {
                    Debug.LogWarning("Raycast не вдарив в об'єкт.");
                    return;
                }

                if (material == null)
                {
                    Debug.LogWarning("Немає PhysicMaterial.");
                }

                AudioClip[] clips;

                if (material != null && materialSounds.TryGetValue(material, out clips))
                {
                    // Material-specific clips found
                }
                else
                {
                    // Use default clips if material is not found
                    clips = defaultClips;
                }

                if (clips.Length > 0)
                {
                    PlayRandomClip(clips);
                }
                else
                {
                    Debug.LogWarning("Немає доступних кліпів для матеріалу.");
                }
            }
            else
            {
                Debug.LogWarning("Raycast не знайде матеріал.");
            }
        }

        private void PlayRandomClip(AudioClip[] clips)
        {
            if (clips == null || clips.Length == 0)
            {
                Debug.LogError("Немає кліпів для відтворення.");
                return;
            }

            AudioClip clip = clips[Random.Range(0, clips.Length)];

            if (clip == null)
            {
                Debug.LogError("Вибраний кліп — null.");
                return;
            }

            audioSource.clip = clip;
            audioSource.Play();
        }

        private IEnumerator RegenerateMana()
        {
            while (true)
            {
                yield return new WaitForSeconds(curManaRegenInterval);
                if (model.Mana < maxMana)
                {
                    int newMana = Mathf.Min(model.Mana + manaRegenAmount, maxMana);
                    model.AddMana(newMana - model.Mana);
                }
            }
        }
        private IEnumerator RegenerateHealth()
        {
            while (true)
            {
                yield return new WaitForSeconds(healthRegenInterval);
                if (model.Health < maxHealth)
                {
                    int newHealth = Mathf.Min(model.Health + healthaRegenAmount, maxHealth);
                    model.AddHealth(newHealth - model.Health);
                }
            }
        }
        public void EnableRegen(float manaRegenInterval, float duration) 
        {
            if (curManaRegenInterval == manaRegenInterval) //if poition had the same buff
            {
                if (timer < 1)
                {
                    curManaRegenInterval = manaRegenInterval;
                    StartCoroutine(WaitForDuration(duration));
                }
                else
                {
                    timer += duration;
                }
            }
            else 
            {
                timer = duration;
                curManaRegenInterval = manaRegenInterval;
                StartCoroutine(WaitForDuration(duration));
            }
        }        
        private IEnumerator WaitForDuration(float duration) 
        {
            timer = duration;
            while (timer >= 0)
            {               
                view.UpdateTimerText(timer);
                yield return new WaitForSeconds(1f);
                timer -= 1;               
            }            
            curManaRegenInterval = manaRegenInterval;
        }
    }
}
