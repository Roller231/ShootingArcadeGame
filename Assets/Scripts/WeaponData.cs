using UnityEngine;

[CreateAssetMenu(fileName = "NewWeapon", menuName = "Weapon System/Weapon")]
public class WeaponData : ScriptableObject
{
    public string weaponName;         // Название оружия
    public float damage;              // Урон
    public int maxAmmo;               // Максимальное количество патронов в магазине
    public float reloadTime;          // Время перезарядки
    public int totalAmmo = 100;       // Общий запас патронов
    public float fireRate = 0.2f;     // Задержка между выстрелами
    public bool isAutomatic = false; // Может ли стрелять при зажатии ЛКМ
    public Sprite icon;
}
