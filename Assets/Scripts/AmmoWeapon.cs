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
        // Проверяем, если игрок вошел в триггер
        if (other.gameObject.tag == "Player")
        {
            isPlayerInTrigger = true;
            canvasButton.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // Проверяем, если игрок покинул триггер
        if (other.gameObject.tag == "Player")
        {
            isPlayerInTrigger = false;
            canvasButton.SetActive(false);

        }
    }

    void Update()
    {
        // Если игрок в триггере и нажал клавишу E
        if (isPlayerInTrigger && Input.GetKeyDown(KeyCode.E))
        {
            // Находим объект с тегом "CanvasPlayer" и получаем соответствующие компоненты
            CrosshairController controller = GameObject.FindGameObjectWithTag("CanvasPlayer").GetComponent<CrosshairController>();

            // Добавляем патроны
            controller.weapons[weaponIndex].totalAmmo += amoToAdd;
            Debug.Log(controller.weapons[weaponIndex].totalAmmo);

            // Сохраняем данные
            GameObject.FindGameObjectWithTag("CanvasPlayer").GetComponent<SavesController>().SaveData();

            // Уничтожаем объект
            Destroy(gameObject);
        }
    }
}
