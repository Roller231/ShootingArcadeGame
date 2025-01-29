using UnityEngine;

public class LookAtPlayer : MonoBehaviour
{
    private Transform player;

    void Start()
    {
        // ���� ������ � ����� "Player"
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    void Update()
    {
        // ���������, ���� ����� ������, ������������ ������, ����� �� ������� �� ������
        if (player != null)
        {
            transform.LookAt(player);
        }
    }
}
