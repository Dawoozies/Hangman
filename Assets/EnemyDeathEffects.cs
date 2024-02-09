using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDeathEffects : MonoBehaviour
{
    public GameObject deathEffectPrefab;
    List<ParticleSystem> activeSystems = new();
    List<ParticleSystem> inactiveSystems = new();
    private void Start()
    {
        EnemySpawnManager.RegisterEnemySpawnCallback((Enemy enemySpawned) => enemySpawned.RegisterEnemyDeadCallback(EnemyDeathHandler));
    }
    private void Update()
    {
        if(activeSystems.Count > 0)
        {
            List<ParticleSystem> newActiveSystems = new();
            foreach (ParticleSystem system in activeSystems)
            {
                if (!system.isPlaying)
                {
                    inactiveSystems.Add(system);
                }
                else
                {
                    newActiveSystems.Add(system);
                }
            }
            activeSystems = newActiveSystems;
        }
    }
    void EnemyDeathHandler(Enemy deadEnemy)
    {
        ParticleSystem system = null;
        if(inactiveSystems.Count == 0)
        {
            GameObject newSystem = Instantiate(deathEffectPrefab);
            system = newSystem.GetComponent<ParticleSystem>();
        }
        else
        {
            system = inactiveSystems[0];
        }
        system.transform.position = deadEnemy.transform.position;
        system.Play(true);
        activeSystems.Add(system);
    }
}
