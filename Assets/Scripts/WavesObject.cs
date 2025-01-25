using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class WavesObject : MonoBehaviour
{
    public List<GameObject> enemysWaves;

    public int nextPoint;


    private void Update()
    {
        CheckNextWave();
    }

    public void CheckNextWave()
    {
        foreach (GameObject enemy in enemysWaves)
        {
            if (enemy.GetComponent<NavMeshAgent>().enabled == true) return;

        }

        NextWave();
        gameObject.SetActive(false);
    }

    public void NextWave()
    {
        for (int i = 0; i < nextPoint; i++)
        {
            GameObject.FindGameObjectWithTag("Player").GetComponent<CameraControllerMover>().StartMove();
            Debug.Log("Start move to next point");
        }
    }

    public void StartWave()
    {
        foreach (GameObject wave in enemysWaves)
        {
            wave.GetComponent<EnemyAI>().enemyActive = true;
        }
    }
}
