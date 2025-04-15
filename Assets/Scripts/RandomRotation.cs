using UnityEngine;

public class RandomRotation : MonoBehaviour
{
    public float rotationSpeed = 50f;  // �������� ���������

    void Update()
    {
        // �������� ���������� ��� ��� ��������� �� ����� ��
        float randomX = Random.Range(-rotationSpeed, rotationSpeed);
        float randomY = Random.Range(-rotationSpeed, rotationSpeed);
        float randomZ = Random.Range(-rotationSpeed, rotationSpeed);

        // �������� ��'���
        transform.Rotate(randomX * Time.deltaTime, randomY * Time.deltaTime, randomZ * Time.deltaTime);
    }
}
