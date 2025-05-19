using Photon.Realtime;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using System.Linq;

namespace LevelGenerator
{
    public class LevelGenerator : MonoBehaviourPun
    {
        [SerializeField] private GameObject[] roomPrefabs;
        //[SerializeField] private GameObject bossRoomPrefab;
        [SerializeField] private GameObject finalRoomPrefab;
        [SerializeField] private GameObject startlRoomPrefab;
        [SerializeField] private GameObject wallRoomPrefab;
        [SerializeField] private int baseRooms = 15;
        [SerializeField] private int branchFrequency = 3; 
        [SerializeField] private int maxBranches = 2; 

        private List<Vector3> usedPositions = new List<Vector3>();
        private List<GameObject> spawnedRooms = new List<GameObject>();

        [SerializeField] private EnemyKillCount enemyKillCount;

        private Vector3[] directions = new Vector3[]
        {
            new Vector3(0, 0, 10),
            new Vector3(10, 0, 0),
            new Vector3(0, 0, -10),
            new Vector3(-10, 0, 0)
        };

        void Start()
        {
            if (PhotonNetwork.IsMasterClient)
            {
                int currentLevel = LevelHandler.Level;
                int maxRooms = baseRooms + (currentLevel - 1);

                //if (currentLevel % 10 == 0)
                //{
                //    PhotonNetwork.Instantiate(bossRoomPrefab.name,Vector3.zero,Quaternion.identity);
                //}
                //else 
                //{
                    GenerateLevel(maxRooms);
                    PlaceFinalRoom();
                    PlaceWalls();
                    photonView.RPC("SynchronizeLevel", RpcTarget.Others, usedPositions.ToArray());
                //}
            }
        }

        void GenerateLevel(int maxRooms)
        {
            Quaternion randomRotation = Quaternion.Euler(0, 90 * Random.Range(0, 4), 0);
           
            GameObject startRoom = PhotonNetwork.Instantiate(
                startlRoomPrefab.name,
                Vector3.zero,
                randomRotation
            );

            usedPositions.Add(Vector3.zero);
            spawnedRooms.Add(startRoom);

            for (int i = 0; i < maxRooms - 1; i++)
            {
                PlaceNextRoom();

                if ((i + 1) % branchFrequency == 0)
                {
                    TryPlaceBranch();
                }
            }
        }

        private void PlaceNextRoom()
        {
            GameObject lastRoom = spawnedRooms[spawnedRooms.Count - 1];
            Vector3 basePosition = lastRoom.transform.position;

            List<Vector3> validPositions = directions
                .Select(d => basePosition + d)
                .Where(pos => !usedPositions.Contains(pos))
                .ToList();

            if (validPositions.Count == 0)
            {
                Debug.LogWarning("No valid positions");
                return;
            }

            Vector3 selectedPosition = validPositions[Random.Range(0, validPositions.Count)];
            Quaternion randomRotation = Quaternion.Euler(0, 90 * Random.Range(0, 4), 0);

            GameObject newRoom = PhotonNetwork.Instantiate(
                roomPrefabs[Random.Range(0, roomPrefabs.Length)].name,
                selectedPosition,
                randomRotation
            );

            if (newRoom.GetComponent<Spawner>()) 
            {
                enemyKillCount.enemiesToKill++;
            }

            usedPositions.Add(selectedPosition);
            spawnedRooms.Add(newRoom);
        }

        private void TryPlaceBranch()
        {
            GameObject branchFromRoom = spawnedRooms[Random.Range(0, spawnedRooms.Count)];
            Vector3 basePosition = branchFromRoom.transform.position;

            List<Vector3> availableBranches = directions
                .Select(d => basePosition + d)
                .Where(pos => !usedPositions.Contains(pos))
                .ToList();

            int branchesToPlace = Mathf.Min(maxBranches, availableBranches.Count);

            for (int i = 0; i < branchesToPlace; i++)
            {
                Vector3 branchPosition = availableBranches[Random.Range(0, availableBranches.Count)];
                availableBranches.Remove(branchPosition);

                Quaternion randomRotation = Quaternion.Euler(0, 90 * Random.Range(0, 4), 0);
                GameObject branchRoom = PhotonNetwork.Instantiate(
                    roomPrefabs[Random.Range(0, roomPrefabs.Length)].name,
                    branchPosition,
                    randomRotation
                );

                usedPositions.Add(branchPosition);
            }
        }

        private void PlaceFinalRoom()
        {
            if (spawnedRooms.Count > 0)
            {
                GameObject lastRoom = spawnedRooms[spawnedRooms.Count - 1];
                Vector3 lastPosition = lastRoom.transform.position;

                PhotonNetwork.Destroy(lastRoom);

                Quaternion randomRotation = Quaternion.Euler(0, 90 * Random.Range(0, 4), 0);
                GameObject finalRoom = PhotonNetwork.Instantiate(
                    finalRoomPrefab.name,
                    lastPosition,
                    randomRotation
                );

                spawnedRooms[spawnedRooms.Count - 1] = finalRoom;

                enemyKillCount.NormalizeEnemyToKill();
            }
        }

        private void PlaceWalls()
        {
            HashSet<Vector3> wallPositions = new HashSet<Vector3>();

            foreach (Vector3 roomPosition in usedPositions)
            {
                foreach (Vector3 direction in directions)
                {
                    Vector3 checkPos = roomPosition + direction;
                    if (!usedPositions.Contains(checkPos) && !wallPositions.Contains(checkPos))
                    {
                        wallPositions.Add(checkPos);
                        PhotonNetwork.Instantiate(wallRoomPrefab.name, checkPos, Quaternion.identity);
                    }
                }
            }
        }

        [PunRPC]
        void SynchronizeLevel(Vector3[] positions)
        {
            usedPositions.AddRange(positions);
        }
    }
}
