using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    public string playerTag = "Player";           // Тег для поиска игрока
    public float attackDistance = 2f;            // Дистанция атаки
    public float attackDamage = 10f;             // Урон от атаки
    public float attackCooldown = 1f;            // Задержка между атаками
    public float delayAgterGetDammage = 1f;     // Задержка между атаками

    private NavMeshAgent agent;                  // AI-навигация
    private Transform player;                    // Ссылка на игрока
    private float lastAttackTime;
    private float agentSpeedSave;

    private bool isAttacking = false;            // Флаг состояния атаки
    private Animator animator;
    public bool canAttack;
    public bool canGetDamage;
    public float delayAfterDamage = 1f;     // Задержка между атаками


    // Новый параметр для игнорирования слоя "Enemy"
    public LayerMask raycastLayerMask;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();

        agentSpeedSave = agent.speed;

        // Поиск игрока по тегу
        GameObject playerObject = GameObject.FindGameObjectWithTag(playerTag);
        if (playerObject != null)
        {
            player = playerObject.transform;
            animator.SetBool("IsRun", true);
        }
        else
        {
            Debug.LogError("Player not found! Make sure the player has the correct tag.");
        }
    }

    private void Update()
    {
        if (player == null || GetComponent<HealthSystem>().currentHealth <= 0) return;

        // Проверяем расстояние до игрока
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        // Если игрок в пределах дистанции атаки, пытаемся атаковать
        if (distanceToPlayer <= attackDistance)
        {
            // Выпускаем луч (Raycast) от врага к игроку
            Ray ray = new Ray(transform.position + Vector3.up, (player.position - transform.position).normalized);
            RaycastHit hit;

            // Визуализация Raycast
            Debug.DrawRay(ray.origin, ray.direction * attackDistance, Color.red);

            // Выполняем Raycast с учетом маски слоев
            if (Physics.Raycast(ray, out hit, attackDistance, raycastLayerMask))
            {
                if (hit.collider.CompareTag(playerTag))
                {
                    if (!isAttacking)
                    {
                        StartCoroutine(AttackPlayer());
                    }
                    return; // Останавливаемся, если атакуем
                }
            }
        }

        // Если не попали лучом или слишком далеко, продолжаем движение
        isAttacking = false;
        agent.isStopped = false;
        agent.SetDestination(player.position);
        animator.SetBool("IsRun", true);
    }

    public IEnumerator AttackPlayer()
    {
        if (canAttack)
        {
            isAttacking = true;
            agent.isStopped = true; // Останавливаем движение

            while (isAttacking)
            {
                animator.SetBool("IsRun", false);

                // Если прошло недостаточно времени с последней атаки, ждем
                if (Time.time - lastAttackTime >= attackCooldown)
                {
                    // Наносим урон игроку
                    HealthSystem playerHealth = player.GetComponent<HealthSystem>();
                    if (playerHealth != null)
                    {
                        if (player == null || GetComponent<HealthSystem>().currentHealth <= 0) break;

                        animator.SetTrigger("Attack");
                        Debug.Log($"Player took {attackDamage} damage. Current health: {playerHealth}");
                    }

                    lastAttackTime = Time.time;
                }

                // Ждем окончания задержки перед следующей атакой
                yield return null;
            }
        }
    }

    public void AttackPlayerFunc()
    {
        player.GetComponent<HealthSystem>().TakeDamage(attackDamage);
    }

    public void GetDamage()
    {
        if (!canAttack || !canGetDamage) return;

        HealthSystem health = GetComponent<HealthSystem>();

        // Проверяем, если у врага достаточно здоровья для получения урона
        if (health.currentHealth > 0)
        {
            // Останавливаем движение врага
            agent.speed = 0f;
            isAttacking = false; // Враг перестает атаковать при получении урона

            // Включаем анимацию получения урона
            animator.SetTrigger("Damage");
            StartCoroutine(delayForDamageAnim());
            // Можем использовать флаг canAttack, чтобы не дать врагу сразу снова атаковать
            canAttack = false;
        }
    }

    public void AfterDamage()
    {
        // После анимации получения урона враг может снова двигаться и атаковать
        canAttack = true;
        agent.speed = agentSpeedSave;

    }

    IEnumerator delayForDamageAnim()
    {
        canGetDamage = false;

        yield return new WaitForSeconds(delayAfterDamage);

        canGetDamage = true;
    }
}
