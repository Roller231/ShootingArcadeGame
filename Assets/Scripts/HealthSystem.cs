using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

public class HealthSystem : MonoBehaviour
{
    public float maxHealth = 100f;
    public float currentHealth;

    private void Start()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(float damage)
    {

            currentHealth -= damage;
        Debug.Log($"{gameObject.name} took {damage} damage. Current health: {currentHealth}");

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        if(gameObject.tag == "Player")
        {
            SceneManager.LoadScene(0);
        }


        Debug.Log($"{gameObject.name} has died!");
        //Destroy(gameObject); // ”ничтожить объект при смерти
        GetComponent<Animator>().SetTrigger("Die");
        GetComponent<Collider>().enabled = false;
        GetComponent<NavMeshAgent>().enabled = false;
        //GetComponent<EnemyAI>().enabled = false;
    }
}
