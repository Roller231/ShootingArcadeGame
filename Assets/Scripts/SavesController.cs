using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class SavesController : MonoBehaviour
{
    public List<WeaponData> weapons; // Список ваших объектов WeaponData.
    private bool firstGame = true;


    private void Awake()
    {

    }

    // Функция сохранения данных в JSON
    public void SaveData()
    {
        
        int i = 0;
        foreach (var weapon in weapons)
        {
            PlayerPrefs.SetInt("CurrentAmo" + i, weapon.currentAmmo);
            PlayerPrefs.SetInt("TotalAmo" + i, weapon.totalAmmo);



            i++;
        }

        PlayerPrefs.SetInt("first", 1);
        PlayerPrefs.Save();
    }

    // Функция загрузки данных из JSON
    public void LoadData()
    {
        if(PlayerPrefs.GetInt("first") == 1) firstGame = false;  
        else firstGame = true;

        if (firstGame)
        {
            int l = 0;
            foreach (var weapon in weapons)
            {
                weapon.currentAmmo = weapon.maxAmmo;
                weapon.totalAmmo = weapon.maxAmmo;
                l++;
            }
        }
        else
        {
            int i = 0;
            foreach (var weapon in weapons)
            {
                weapon.currentAmmo = PlayerPrefs.GetInt("CurrentAmo" + i);
                weapon.totalAmmo = PlayerPrefs.GetInt("TotalAmo" + i);
                i++;
            }
        }

    }
}
