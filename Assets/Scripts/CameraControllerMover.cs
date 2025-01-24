using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControllerMover : MonoBehaviour
{

    public Transform[] waypoints; // ������ ����� ��������
    public float moveSpeed = 5f;  // �������� �������� ������
    public float rotationSpeed = 2f; // �������� �������� ������ � �����

    [SerializeField] public Animator animator;

    private int currentWaypointIndex = 0;
    private bool isMoving = false;

    private void Start()
    {
        if (waypoints.Length > 0)
        {
            // ������������� ��������� ������� � ���������� ������
            transform.position = waypoints[0].position;
            transform.rotation = waypoints[0].rotation;
        }
    }

    private void Update()
    {
        // ��������� ������� ����� ������ ���� � ��������� �������� � ��������� �����
        if (Input.GetMouseButtonDown(1) && !isMoving && waypoints.Length > 0)
        {
            StartCoroutine(MoveToNextWaypoint());
        }
    }

    private IEnumerator MoveToNextWaypoint()
    {
        animator.SetBool("IsWalking", true);

        isMoving = true;

        // ������� �����
        Transform targetWaypoint = waypoints[currentWaypointIndex];
        Waypoint waypointData = targetWaypoint.GetComponent<Waypoint>();

        // �������� ������ � �����
        while (Vector3.Distance(transform.position, targetWaypoint.position) > 0.1f ||
               Quaternion.Angle(transform.rotation, targetWaypoint.rotation) > 0.1f)
        {
            // ������� ����������� � �������
            transform.position = Vector3.MoveTowards(
                transform.position,
                targetWaypoint.position,
                moveSpeed * Time.deltaTime
            );

            // ������� �������� � �������� ����������
            transform.rotation = Quaternion.Lerp(
                transform.rotation,
                waypointData != null ? waypointData.targetRotation : targetWaypoint.rotation,
                rotationSpeed * Time.deltaTime
            );

            yield return null;
        }

        // ������������ �� ��������� �����
        currentWaypointIndex = (currentWaypointIndex + 1) % waypoints.Length;
        isMoving = false;
        animator.SetBool("IsWalking", false);

    }
}


