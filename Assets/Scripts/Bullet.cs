using UnityEngine;

public class Bullet : MonoBehaviour
{
    // Пуля будет просто лететь и исчезать через время
    private void Start()
    {
        // Можно добавить логику визуализации, например, анимацию
    }

    private void Update()
    {
        // Логика для движения пули
        // Визуализация или эффекты можно добавить здесь
    }

    // Уничтожение пули
    private void OnCollisionEnter(Collision collision)
    {
        // Уничтожаем пулю при столкновении
        Destroy(gameObject);
    }
}
