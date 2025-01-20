using UnityEngine;

[CreateAssetMenu(fileName = "NewWeapon", menuName = "Weapon System/Weapon")]
public class WeaponData : ScriptableObject
{
    public string weaponName;         // �������� ������
    public float damage;              // ����
    public int maxAmmo;               // ������������ ���������� �������� � ��������
    public float reloadTime;          // ����� �����������
    public int totalAmmo = 100;       // ����� ����� ��������
    public float fireRate = 0.2f;     // �������� ����� ����������
    public bool isAutomatic = false; // ����� �� �������� ��� ������� ���
    public Sprite icon;
}
