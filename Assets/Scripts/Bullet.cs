using UnityEngine;

public class Bullet : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        // Если пуля сталкивается с объектом, уничтожаем её
        Destroy(gameObject);

        // Можно добавить дополнительные проверки для уничтожения пули только при столкновении с врагами
        // Например, если объект имеет компонент HealthSystem, то мы наносим урон
        HealthSystem health = collision.gameObject.GetComponent<HealthSystem>();
        if (health != null)
        {
            health.TakeDamage(10f); // Наносим урон
            collision.gameObject.GetComponent<EnemyAI>().GetDamage();

        }

    }
}
