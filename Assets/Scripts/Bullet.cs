using UnityEngine;

public class Bullet : MonoBehaviour
{
    // ���� ����� ������ ������ � �������� ����� �����
    private void Start()
    {
        // ����� �������� ������ ������������, ��������, ��������
    }

    private void Update()
    {
        // ������ ��� �������� ����
        // ������������ ��� ������� ����� �������� �����
    }

    // ����������� ����
    private void OnCollisionEnter(Collision collision)
    {
        // ���������� ���� ��� ������������
        Destroy(gameObject);
    }
}
