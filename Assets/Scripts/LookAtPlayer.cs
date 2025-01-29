using UnityEngine;

public class LookAtPlayer : MonoBehaviour
{
    private Transform player;

    void Start()
    {
        // Ищем объект с тегом "Player"
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    void Update()
    {
        // Проверяем, если игрок найден, поворачиваем объект, чтобы он смотрел на игрока
        if (player != null)
        {
            transform.LookAt(player);
        }
    }
}
