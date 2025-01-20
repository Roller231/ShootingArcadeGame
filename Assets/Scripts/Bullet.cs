using UnityEngine;

public class Bullet : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        // ���� ���� ������������ � ��������, ���������� �
        Destroy(gameObject);

        // ����� �������� �������������� �������� ��� ����������� ���� ������ ��� ������������ � �������
        // ��������, ���� ������ ����� ��������� HealthSystem, �� �� ������� ����
        HealthSystem health = collision.gameObject.GetComponent<HealthSystem>();
        if (health != null)
        {
            health.TakeDamage(10f); // ������� ����
            collision.gameObject.GetComponent<EnemyAI>().GetDamage();

        }

    }
}
