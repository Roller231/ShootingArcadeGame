using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class CrosshairController : MonoBehaviour
{
    public RectTransform crosshair;       // UI-������� �������
    public Camera mainCamera;             // ������, �� ������� ����������� ���
    public WeaponData[] weapons;          // ������ ���� ������
    public int currentWeaponIndex = 0;    // ������� ������ ���������� ������
    public GameObject bulletPrefab;       // ������ ����
    public Transform gunBarrel;           // �����, �� ������� ���� ����� �������� (��������, �� ������ ������)
    public float bulletSpeed = 20f;       // �������� ����

    [SerializeField] private int currentAmmoInClip; // ������� ���������� �������� � �������� (������������� ����������)
    private bool isReloading = false;     // ���� �����������
    private bool isShooting = false;      // ���� ��������
    private float lastShotTime;           // ����� ���������� ��������

    [SerializeField] private Text ammoText;
    [SerializeField] private Image iconWeapon;


    private void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Confined;

        EquipWeapon(currentWeaponIndex);
    }

    private void Update()
    {
        UpdateCrosshairPosition();
        UpdateUI();

        // ����� ������
        if (Input.GetKeyDown(KeyCode.Alpha1)) EquipWeapon(0);
        if (Input.GetKeyDown(KeyCode.Alpha2)) EquipWeapon(1);
        if (Input.GetKeyDown(KeyCode.Alpha3)) EquipWeapon(2);

        // ��������� ��������
        if (!isReloading)
        {
            if (weapons[currentWeaponIndex].isAutomatic)
            {
                if (Input.GetMouseButton(0) && !isShooting)
                {
                    StartCoroutine(HandleAutomaticShooting());
                }
            }
            else
            {
                if (Input.GetMouseButtonDown(0) && !isShooting)
                {
                    StartCoroutine(HandleSingleShot());
                }
            }
        }

        // �����������
        if (Input.GetKeyDown(KeyCode.R) && !isReloading)
        {
            StartCoroutine(ReloadWeapon());
        }
    }

    private void UpdateCrosshairPosition()
    {
        Vector3 mousePosition = Input.mousePosition;
        crosshair.position = mousePosition;
    }

    private IEnumerator HandleSingleShot()
    {
        isShooting = true;

        if (Time.time - lastShotTime >= weapons[currentWeaponIndex].fireRate)
        {
            Shoot();
            lastShotTime = Time.time;
        }

        isShooting = false;
        yield return null;
    }

    private IEnumerator HandleAutomaticShooting()
    {
        isShooting = true;

        while (Input.GetMouseButton(0))
        {
            if (Time.time - lastShotTime >= weapons[currentWeaponIndex].fireRate)
            {
                Shoot();
                lastShotTime = Time.time;
            }
            yield return null;
        }

        isShooting = false;
    }

    private void Shoot()
    {
        if (currentAmmoInClip <= 0)
        {
            Debug.Log("Out of ammo in the clip! Reload your weapon.");
            return;
        }

        // ������� ���� � ��������� �
        FireBullet();

        currentAmmoInClip--; // ��������� ���������� ��������
        Debug.Log($"Shot fired! Remaining ammo in clip: {currentAmmoInClip}. Total ammo left: {weapons[currentWeaponIndex].totalAmmo}");
    }

    private void FireBullet()
    {
        // ������� ���� ��� ���������� ������
        GameObject bullet = Instantiate(bulletPrefab, gunBarrel.position, gunBarrel.rotation);
        Rigidbody bulletRb = bullet.GetComponent<Rigidbody>();

        if (bulletRb != null)
        {
            // ����������� ���� ������������ ����� Raycast
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            // ����������� ���� ����� �������� �� ������ � ����� ��������� �������
            Vector3 direction = ray.direction;

            // ������������� �������� ����
            bulletRb.velocity = direction * bulletSpeed;

            // ��������� ��������� � ������� Raycast
            if (Physics.Raycast(ray, out hit))
            {
                // ���������, ���� �� ������ � �����
                if (hit.collider.CompareTag("Enemy"))
                {
                    // ������� ���� �����
                    HealthSystem health = hit.collider.GetComponent<HealthSystem>();
                    if (health != null)
                    {
                        health.TakeDamage(10f); // ������� ����
                        hit.collider.GetComponent<EnemyAI>().GetDamage(); // ����� ����� �������� ������ �����
                    }
                }
            }
        }

        // ���������� ���� ����� ��������� �����
        Destroy(bullet, 3f); // ���� �������� ����� 3 �������
    }

    private IEnumerator ReloadWeapon()
    {
        isReloading = true;
        Debug.Log("Reloading...");
        yield return new WaitForSeconds(weapons[currentWeaponIndex].reloadTime);

        int ammoNeeded = weapons[currentWeaponIndex].maxAmmo - currentAmmoInClip;
        int ammoToReload = Mathf.Min(ammoNeeded, weapons[currentWeaponIndex].totalAmmo);

        currentAmmoInClip += ammoToReload;
        weapons[currentWeaponIndex].totalAmmo -= ammoToReload;

        Debug.Log($"Reload complete! Ammo in clip: {currentAmmoInClip}. Total ammo left: {weapons[currentWeaponIndex].totalAmmo}");
        isReloading = false;
    }

    private void EquipWeapon(int index)
    {
        if (index < 0 || index >= weapons.Length)
        {
            Debug.LogError("Invalid weapon index!");
            return;
        }

        currentWeaponIndex = index;
        currentAmmoInClip = weapons[currentWeaponIndex].maxAmmo;


        Debug.Log($"Equipped weapon: {weapons[currentWeaponIndex].weaponName}. Ammo in clip: {currentAmmoInClip}. Total ammo: {weapons[currentWeaponIndex].totalAmmo}");
    }

    private void UpdateUI()
    {
        ammoText.text = currentAmmoInClip + "/" + weapons[currentWeaponIndex].totalAmmo;
        iconWeapon.sprite = weapons[currentWeaponIndex].icon;
    }
}
