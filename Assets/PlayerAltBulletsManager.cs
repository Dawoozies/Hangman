using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAltBulletsManager : MonoBehaviour
{
    public Player player;
    public GameObject bulletExplosion;
    List<ParticleSystem> activeBulletExplosions = new();
    List<ParticleSystem> inactiveBulletExplosions = new();
    int explosiveEnemiesRank = 0;

    public GameObject bouncingBulletPrefab;
    public List<BouncingBullet> bouncingBulletsNormal = new();
    float bouncingBulletTimer = 0f;

    void Start()
    {
        GameManager.RegisterUpgradeCollectCallback(UpgradeCollectHandler);
        EnemySpawnManager.RegisterEnemyDeadCallback(EnemyDeadHandler);
        explosiveEnemiesRank = 0;
    }
    void Update()
    {
        List<ParticleSystem> completeSystems = new();
        foreach (ParticleSystem system in activeBulletExplosions)
        {
            if(!system.isPlaying)
            {
                completeSystems.Add(system);
            }
        }
        foreach (ParticleSystem system in completeSystems)
        {
            inactiveBulletExplosions.Add(system);
            activeBulletExplosions.Remove(system);
        }
        if(bouncingBulletsNormal != null && bouncingBulletsNormal.Count > 0)
        {
            if (bouncingBulletTimer > 5f)
            {
                foreach (BouncingBullet bullet in bouncingBulletsNormal)
                {
                    bullet.ResetBullet(player.transform.position);
                }
                bouncingBulletTimer = 0f;
            }
        }
    }
    void UpgradeCollectHandler(Upgrade upgradeCollected, int rank)
    {
        if(upgradeCollected.upgradeFlag.HasFlag(UpgradeFlag.ExplosiveEnemies))
        {
            explosiveEnemiesRank = rank;
        }
        if(upgradeCollected.upgradeFlag.HasFlag(UpgradeFlag.BounceShotNormal))
        {
            int toSpawn = rank - bouncingBulletsNormal.Count;
            for (int i = 0; i < toSpawn; i++)
            {
                GameObject bouncingBulletObject = Instantiate(bouncingBulletPrefab);
                BouncingBullet bouncingBullet = bouncingBulletObject.GetComponent<BouncingBullet>();
                bouncingBulletsNormal.Add(bouncingBullet);
            }
        }
    }
    void EnemyDeadHandler(Enemy deadEnemy)
    {
        if(explosiveEnemiesRank > 0)
        {
            ParticleSystem system = null;
            if(inactiveBulletExplosions.Count == 0)
            {
                GameObject newObject = Instantiate(bulletExplosion);
                system = newObject.GetComponent<ParticleSystem>();
            }
            else
            {
                system = inactiveBulletExplosions[0];
            }
            if(system != null)
            {
                int particles = 10 * explosiveEnemiesRank;
                Debug.Log("Particles = " + particles);
                var main = system.main;
                main.maxParticles = particles;
                var emission = system.emission;
                emission.SetBurst(0, new(0f, particles));
                system.Play();
                activeBulletExplosions.Add(system);
                system.transform.position = deadEnemy.transform.position;
            }
        }
    }
}
