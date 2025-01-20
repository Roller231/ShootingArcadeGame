using UnityEngine;

public class Waypoint : MonoBehaviour
{
    public string pointName; // ��� �����
    public float waitTime;   // ����� �������� �� �����

    public Quaternion targetRotation; // ���������� ������

    private void Awake()
    {
        // ��������� ������� ���������� ����� ��� �������
        targetRotation = transform.rotation;
    }
}
