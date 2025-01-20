using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class CrosshairController : MonoBehaviour
{
    public RectTransform crosshair;       // UI-элемент прицела
    public Camera mainCamera;             // Камера, из которой выпускается луч
    public WeaponData[] weapons;          // Список всех оружий
    public int currentWeaponIndex = 0;    // Текущий индекс выбранного оружия
    public GameObject bulletPrefab;       // Префаб пули
    public Transform gunBarrel;           // Точка, из которой пуля будет вылетать (например, из ствола оружия)
    public float bulletSpeed = 20f;       // Скорость пули

    [SerializeField] private int currentAmmoInClip; // Текущее количество патронов в магазине (сериализуемая переменная)
    private bool isReloading = false;     // Флаг перезарядки
    private bool isShooting = false;      // Флаг стрельбы
    private float lastShotTime;           // Время последнего выстрела

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

        // Смена оружия
        if (Input.GetKeyDown(KeyCode.Alpha1)) EquipWeapon(0);
        if (Input.GetKeyDown(KeyCode.Alpha2)) EquipWeapon(1);
        if (Input.GetKeyDown(KeyCode.Alpha3)) EquipWeapon(2);

        // Обработка стрельбы
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

        // Перезарядка
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

        // Создаем пулю и запускаем её
        FireBullet();

        currentAmmoInClip--; // Уменьшаем количество патронов
        Debug.Log($"Shot fired! Remaining ammo in clip: {currentAmmoInClip}. Total ammo left: {weapons[currentWeaponIndex].totalAmmo}");
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

            // Проверяем попадание с помощью Raycast
            if (Physics.Raycast(ray, out hit))
            {
                // Проверяем, если мы попали в врага
                if (hit.collider.CompareTag("Enemy"))
                {
                    // Наносим урон врагу
                    HealthSystem health = hit.collider.GetComponent<HealthSystem>();
                    if (health != null)
                    {
                        health.TakeDamage(10f); // Наносим урон
                        hit.collider.GetComponent<EnemyAI>().GetDamage(); // Также можно вызывать логику врага
                    }
                }
            }
        }

        // Уничтожаем пулю через некоторое время
        Destroy(bullet, 3f); // Пуля исчезает через 3 секунды
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
