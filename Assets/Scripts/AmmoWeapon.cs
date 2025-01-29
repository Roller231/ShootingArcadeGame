using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmmoWeapon : MonoBehaviour
{
    public int amoToAdd;
    public int weaponIndex;

    private bool isPlayerInTrigger = false;

    [SerializeField] private GameObject canvasButton;

    private void OnTriggerEnter(Collider other)
    {
        // ���������, ���� ����� ����� � �������
        if (other.gameObject.tag == "Player")
        {
            isPlayerInTrigger = true;
            canvasButton.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // ���������, ���� ����� ������� �������
        if (other.gameObject.tag == "Player")
        {
            isPlayerInTrigger = false;
            canvasButton.SetActive(false);

        }
    }

    void Update()
    {
        // ���� ����� � �������� � ����� ������� E
        if (isPlayerInTrigger && Input.GetKeyDown(KeyCode.E))
        {
            // ������� ������ � ����� "CanvasPlayer" � �������� ��������������� ����������
            CrosshairController controller = GameObject.FindGameObjectWithTag("CanvasPlayer").GetComponent<CrosshairController>();

            // ��������� �������
            controller.weapons[weaponIndex].totalAmmo += amoToAdd;
            Debug.Log(controller.weapons[weaponIndex].totalAmmo);

            // ��������� ������
            GameObject.FindGameObjectWithTag("CanvasPlayer").GetComponent<SavesController>().SaveData();

            // ���������� ������
            Destroy(gameObject);
        }
    }
}
