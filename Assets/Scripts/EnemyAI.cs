using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    public string playerTag = "Player";           // ��� ��� ������ ������
    public float attackDistance = 2f;            // ��������� �����
    public float attackDamage = 10f;             // ���� �� �����
    public float attackCooldown = 1f;            // �������� ����� �������
    public float delayAgterGetDammage = 1f;     // �������� ����� �������

    private NavMeshAgent agent;                  // AI-���������
    private Transform player;                    // ������ �� ������
    private float lastAttackTime;
    private float agentSpeedSave;

    private bool isAttacking = false;            // ���� ��������� �����
    private Animator animator;
    public bool canAttack;
    public bool canGetDamage;
    public float delayAfterDamage = 1f;     // �������� ����� �������


    // ����� �������� ��� ������������� ���� "Enemy"
    public LayerMask raycastLayerMask;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();

        agentSpeedSave = agent.speed;

        // ����� ������ �� ����
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

        // ��������� ���������� �� ������
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        // ���� ����� � �������� ��������� �����, �������� ���������
        if (distanceToPlayer <= attackDistance)
        {
            // ��������� ��� (Raycast) �� ����� � ������
            Ray ray = new Ray(transform.position + Vector3.up, (player.position - transform.position).normalized);
            RaycastHit hit;

            // ������������ Raycast
            Debug.DrawRay(ray.origin, ray.direction * attackDistance, Color.red);

            // ��������� Raycast � ������ ����� �����
            if (Physics.Raycast(ray, out hit, attackDistance, raycastLayerMask))
            {
                if (hit.collider.CompareTag(playerTag))
                {
                    if (!isAttacking)
                    {
                        StartCoroutine(AttackPlayer());
                    }
                    return; // ���������������, ���� �������
                }
            }
        }

        // ���� �� ������ ����� ��� ������� ������, ���������� ��������
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
            agent.isStopped = true; // ������������� ��������

            while (isAttacking)
            {
                animator.SetBool("IsRun", false);

                // ���� ������ ������������ ������� � ��������� �����, ����
                if (Time.time - lastAttackTime >= attackCooldown)
                {
                    // ������� ���� ������
                    HealthSystem playerHealth = player.GetComponent<HealthSystem>();
                    if (playerHealth != null)
                    {
                        if (player == null || GetComponent<HealthSystem>().currentHealth <= 0) break;

                        animator.SetTrigger("Attack");
                        Debug.Log($"Player took {attackDamage} damage. Current health: {playerHealth}");
                    }

                    lastAttackTime = Time.time;
                }

                // ���� ��������� �������� ����� ��������� ������
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

        // ���������, ���� � ����� ���������� �������� ��� ��������� �����
        if (health.currentHealth > 0)
        {
            // ������������� �������� �����
            agent.speed = 0f;
            isAttacking = false; // ���� ��������� ��������� ��� ��������� �����

            // �������� �������� ��������� �����
            animator.SetTrigger("Damage");
            StartCoroutine(delayForDamageAnim());
            // ����� ������������ ���� canAttack, ����� �� ���� ����� ����� ����� ���������
            canAttack = false;
        }
    }

    public void AfterDamage()
    {
        // ����� �������� ��������� ����� ���� ����� ����� ��������� � ���������
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
