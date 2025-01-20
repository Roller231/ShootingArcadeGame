using UnityEngine;

public class Waypoint : MonoBehaviour
{
    public string pointName; // Имя точки
    public float waitTime;   // Время ожидания на точке

    public Quaternion targetRotation; // Ориентация камеры

    private void Awake()
    {
        // Сохраняем текущую ориентацию точки как целевую
        targetRotation = transform.rotation;
    }
}
