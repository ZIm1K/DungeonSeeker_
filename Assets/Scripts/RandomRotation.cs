using UnityEngine;

public class RandomRotation : MonoBehaviour
{
    public float rotationSpeed = 50f;  // Швидкість обертання

    void Update()
    {
        // Генеруємо випадковий кут для обертання по кожній осі
        float randomX = Random.Range(-rotationSpeed, rotationSpeed);
        float randomY = Random.Range(-rotationSpeed, rotationSpeed);
        float randomZ = Random.Range(-rotationSpeed, rotationSpeed);

        // Обертаємо об'єкт
        transform.Rotate(randomX * Time.deltaTime, randomY * Time.deltaTime, randomZ * Time.deltaTime);
    }
}
