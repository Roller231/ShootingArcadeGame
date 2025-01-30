using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;

public class CrosshairController : MonoBehaviour
{
    public RectTransform crosshair;
    public Camera mainCamera;
    public WeaponData[] weapons;
    public int currentWeaponIndex = 0;
    public GameObject bulletPrefab;
    public Transform gunBarrel;
    public float bulletSpeed = 20f;

    private bool isReloading = false;
    private bool isShooting = false;
    private float lastShotTime;

    [SerializeField] private Text ammoText;
    [SerializeField] private Image iconWeapon;

    [SerializeField] private GameObject prefabVFXblood;
    [SerializeField] private Transform spawnPointBlood;
    [SerializeField] private SavesController saves;

    private void Start()
    {
        saves.LoadData();
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Confined;

        spawnPointBlood = null;

        EquipWeapon(currentWeaponIndex);
        
    }

    private void Update()
    {
        UpdateCrosshairPosition();
        UpdateUI();

        if(!isReloading)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1) && currentWeaponIndex != 0) EquipWeapon(0);
            if (Input.GetKeyDown(KeyCode.Alpha2) && currentWeaponIndex != 1) EquipWeapon(1);
            if (Input.GetKeyDown(KeyCode.Alpha3) && currentWeaponIndex != 2) EquipWeapon(2);
            if (Input.GetKeyDown(KeyCode.Alpha4) && currentWeaponIndex != 3) EquipWeapon(3);
            if (Input.GetKeyDown(KeyCode.Alpha5) && currentWeaponIndex != 4) EquipWeapon(4);
        }


        if (!isReloading)
        {
            if (weapons[currentWeaponIndex].isAutomatic)
            {
                if (Input.GetMouseButton(0) && !isShooting)
                {
                    StartCoroutine(HandleAutomaticShooting());
                    Cursor.visible = false;
                    Cursor.lockState = CursorLockMode.Confined;
                }
            }
            else
            {
                if (Input.GetMouseButtonDown(0) && !isShooting)
                {
                    StartCoroutine(HandleSingleShot());
                    Cursor.visible = false;
                    Cursor.lockState = CursorLockMode.Confined;
                }
            }
        }

        if (Input.GetKeyDown(KeyCode.R) && !isReloading && weapons[currentWeaponIndex].currentAmmo != weapons[currentWeaponIndex].maxAmmo && weapons[currentWeaponIndex].totalAmmo != 0)
        {
            StartCoroutine(ReloadWeapon());
        }
    }

    private void UpdateCrosshairPosition()
    {
        crosshair.position = Input.mousePosition;
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
        if (weapons[currentWeaponIndex].currentAmmo <= 0)
        {
            Debug.Log("Out of ammo in the clip! Reload your weapon.");
            return;
        }

        FireBullet();
        weapons[currentWeaponIndex].currentAmmo--;
        GetComponent<AudioSource>().clip = weapons[currentWeaponIndex].soundWeaponShoot;
        GetComponent<AudioSource>().Play();
        saves.SaveData();

        Debug.Log($"Shot fired! Ammo in clip: {weapons[currentWeaponIndex].currentAmmo}. Total ammo left: {weapons[currentWeaponIndex].totalAmmo}");
    }

    private void FireBullet()
    {
        // Создаем пулю как визуальный объект
        GameObject bullet = Instantiate(bulletPrefab, gunBarrel.position, gunBarrel.rotation);
        Rigidbody bulletRb = bullet.GetComponent<Rigidbody>();

        if (bulletRb != null)
        {
            // Направление пули определяется через Raycast
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            // Направление пули будет исходить из камеры в точку попадания курсора
            Vector3 direction = ray.direction;

            // Устанавливаем скорость пули
            bulletRb.velocity = direction * bulletSpeed;

            // Определяем LayerMask, чтобы игнорировать слой Weapon
            int layerMask = ~(1 << LayerMask.NameToLayer("Weapon"));  // Игнорируем слой Weapon

            // Проверяем попадание с помощью Raycast, исключая объекты на слое Weapon
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask))
            {
                // Проверяем, если мы попали в врага
                if (hit.collider.CompareTag("Enemy"))
                {
                    // Наносим урон врагу
                    HealthSystem health = hit.collider.GetComponent<HealthSystem>();
                    if (health != null)
                    {
                        health.TakeDamage(weapons[currentWeaponIndex].damage); // Наносим урон
                        hit.collider.GetComponent<EnemyAI>().GetDamage(); // Также можно вызывать логику врага

                        GetComponent<UtilScripts>().PlaySound(hit.collider.GetComponent<AudioSource>());

                        try
                        {
                            spawnPointBlood = hit.collider.gameObject.GetComponentInChildren<spawnPoint>().transform;
                        }
                        catch (NullReferenceException)
                        {
                            spawnPointBlood = null;
                        }

                        if (spawnPointBlood == null)
                        {
                            var prefab = Instantiate(prefabVFXblood, health.gameObject.transform);
                            Destroy(prefab, 4f);
                        }
                        else
                        {
                            var prefab = Instantiate(prefabVFXblood, spawnPointBlood);
                            Destroy(prefab, 4f);
                        }
                    }
                }
            }
        }

        // Уничтожаем пулю через некоторое время
        Destroy(bullet, 3f); // Пуля исчезает через 3 секунды
    }



    private IEnumerator ReloadWeapon()
    {

        GetComponent<AudioSource>().clip = weapons[currentWeaponIndex].soundWeaponReload;
        GetComponent<AudioSource>().Play();

        isReloading = true;
        Debug.Log("Reloading...");
        yield return new WaitForSeconds(weapons[currentWeaponIndex].reloadTime);

        int ammoNeeded = weapons[currentWeaponIndex].maxAmmo - weapons[currentWeaponIndex].currentAmmo;
        int ammoToReload = Mathf.Min(ammoNeeded, weapons[currentWeaponIndex].totalAmmo);

        weapons[currentWeaponIndex].currentAmmo += ammoToReload;
        weapons[currentWeaponIndex].totalAmmo -= ammoToReload;

        Debug.Log($"Reload complete! Ammo in clip: {weapons[currentWeaponIndex].currentAmmo}. Total ammo left: {weapons[currentWeaponIndex].totalAmmo}");
        isReloading = false;

        saves.SaveData();
    }

    private void EquipWeapon(int index)
    {



        if (index < 0 || index >= weapons.Length)
        {
            Debug.LogError("Invalid weapon index!");
            return;
        }
        saves.LoadData();

        currentWeaponIndex = index;
        Debug.Log($"Equipped weapon: {weapons[currentWeaponIndex].weaponName}. Ammo in clip: {weapons[currentWeaponIndex].currentAmmo}. Total ammo: {weapons[currentWeaponIndex].totalAmmo}");
    }

    private void UpdateUI()
    {
        ammoText.text = weapons[currentWeaponIndex].currentAmmo + "/" + weapons[currentWeaponIndex].totalAmmo;
        iconWeapon.sprite = weapons[currentWeaponIndex].icon;
    }
}
