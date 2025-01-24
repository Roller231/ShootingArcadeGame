using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControllerMover : MonoBehaviour
{

    public Transform[] waypoints; // Массив точек маршрута
    public float moveSpeed = 5f;  // Скорость движения камеры
    public float rotationSpeed = 2f; // Скорость вращения камеры к точке

    [SerializeField] public Animator animator;

    private int currentWaypointIndex = 0;
    private bool isMoving = false;

    private void Start()
    {
        if (waypoints.Length > 0)
        {
            // Устанавливаем начальную позицию и ориентацию камеры
            transform.position = waypoints[0].position;
            transform.rotation = waypoints[0].rotation;
        }
    }

    private void Update()
    {
        // Проверяем нажатие левой кнопки мыши и запускаем движение к следующей точке
        if (Input.GetMouseButtonDown(1) && !isMoving && waypoints.Length > 0)
        {
            StartCoroutine(MoveToNextWaypoint());
        }
    }

    private IEnumerator MoveToNextWaypoint()
    {
        animator.SetBool("IsWalking", true);

        isMoving = true;

        // Целевая точка
        Transform targetWaypoint = waypoints[currentWaypointIndex];
        Waypoint waypointData = targetWaypoint.GetComponent<Waypoint>();

        // Движение камеры к точке
        while (Vector3.Distance(transform.position, targetWaypoint.position) > 0.1f ||
               Quaternion.Angle(transform.rotation, targetWaypoint.rotation) > 0.1f)
        {
            // Плавное перемещение к позиции
            transform.position = Vector3.MoveTowards(
                transform.position,
                targetWaypoint.position,
                moveSpeed * Time.deltaTime
            );

            // Плавное вращение к заданной ориентации
            transform.rotation = Quaternion.Lerp(
                transform.rotation,
                waypointData != null ? waypointData.targetRotation : targetWaypoint.rotation,
                rotationSpeed * Time.deltaTime
            );

            yield return null;
        }

        // Переключение на следующую точку
        currentWaypointIndex = (currentWaypointIndex + 1) % waypoints.Length;
        isMoving = false;
        animator.SetBool("IsWalking", false);

    }
}


