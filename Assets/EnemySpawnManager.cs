using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
public class EnemySpawnManager : MonoBehaviour
{
    public float spawnDistance;
    int enemyLimit;
    public int waveDifficulty;
    public GameObject enemyPrefab;
    public float waveTime;
    float waveTimer;
    public List<Enemy> activeEnemies = new();
    List<Enemy> deadEnemies = new();
    Camera mainCamera;
    public Player player;
    static List<Action<Enemy>> enemySpawnActions = new();
    public static List<Action<Enemy>> enemyDeadActions = new();
    void Start()
    {
        mainCamera = Camera.main;
    }
    public void ResetGame()
    {
        waveDifficulty = 1;
        deadEnemies.AddRange(activeEnemies);
        activeEnemies.Clear();
        foreach (Enemy enemy in deadEnemies)
        {
            enemy.transform.position = GenerateSpawnLocation();
            enemy.gameObject.SetActive(false);
        }
    }
    void Update()
    {
        if (GameManager.upgradeScreenActive)
            return;
        float difficultyTimeReduction = 0f;
        float enemyMoveSpeedMultiplier = 1f;
        
        if(waveDifficulty > 3)
        {
            difficultyTimeReduction = waveTime * 0.25f;
            enemyMoveSpeedMultiplier = 2f;
        }
        if(waveDifficulty > 7)
        {
            difficultyTimeReduction = waveTime * 0.5f;
            enemyMoveSpeedMultiplier = 4f;
        }
        if(waveDifficulty > 9)
        {
            difficultyTimeReduction = waveTime * 0.9f;
            enemyMoveSpeedMultiplier = 5f;
        }
        enemyLimit = waveDifficulty * 4;
        if(activeEnemies.Count <= enemyLimit)
        {
            waveTimer += Time.deltaTime;
        }
        if (waveTimer > waveTime - difficultyTimeReduction)
        {
            waveTimer = 0;
            Vector3 spawnLocation = GenerateSpawnLocation();
            if(deadEnemies == null || deadEnemies.Count == 0)
            {
                GameObject newEnemyObject = Instantiate(enemyPrefab, spawnLocation, Quaternion.identity, transform);
                Enemy enemy = newEnemyObject.GetComponent<Enemy>();
                enemy.moveSpeedMultiplier = enemyMoveSpeedMultiplier;
                enemy.player = player;
                activeEnemies.Add(enemy);
                foreach (Action<Enemy> action in enemySpawnActions)
                {
                    action(enemy);
                }
                enemy.RegisterEnemyDeadCallback(EnemyDeadHandler);
            }
            else
            {
                Enemy selectedEnemy = deadEnemies[Random.Range(0, deadEnemies.Count)];
                selectedEnemy.gameObject.SetActive(true);
                selectedEnemy.moveSpeedMultiplier = enemyMoveSpeedMultiplier;
                selectedEnemy.ReviveAtLocation(spawnLocation);
                activeEnemies.Add(selectedEnemy);
                deadEnemies.Remove(selectedEnemy);
            }
        }
    }
    Vector3 GenerateSpawnLocation()
    {
        float cameraWidth = mainCamera.pixelWidth;
        float cameraHeight = mainCamera.pixelHeight;
        Vector2 boundaryCornerA = (Vector2)mainCamera.ScreenToWorldPoint(Vector2.zero);
        Vector2 boundaryCornerB = (Vector2)mainCamera.ScreenToWorldPoint(new Vector2(cameraWidth, cameraHeight));
        float boundaryLengthHalf = Mathf.Abs(boundaryCornerB.x - boundaryCornerA.x)/2f;
        float boundaryHeightHalf = Mathf.Abs(boundaryCornerB.y - boundaryCornerA.y)/2f;
        Vector3 spawnLocation = new Vector3(0, 0, 0);
        int screenSideToSpawnIn = Random.Range(0, 4);
        if (screenSideToSpawnIn == 0)
        {
            //right of screen
            spawnLocation.x = boundaryCornerB.x + spawnDistance;
            spawnLocation.y = Random.Range(-boundaryHeightHalf, boundaryHeightHalf);
        }
        if(screenSideToSpawnIn == 1)
        {
            //left of screen
            spawnLocation.x = boundaryCornerA.x - spawnDistance;
            spawnLocation.y = Random.Range(-boundaryHeightHalf, boundaryHeightHalf);
        }
        if (screenSideToSpawnIn == 2)
        {
            //bottom of screen
            spawnLocation.y = boundaryCornerA.y - spawnDistance;
            spawnLocation.x = Random.Range(-boundaryLengthHalf, boundaryLengthHalf);
        }
        if (screenSideToSpawnIn == 3)
        {
            //top of screen
            spawnLocation.y = boundaryCornerB.y + spawnDistance;
            spawnLocation.x = Random.Range(-boundaryLengthHalf, boundaryLengthHalf);
        }
        return spawnLocation;
    }
    void EnemyDeadHandler(Enemy deadEnemy)
    {
        foreach (Action<Enemy> action in enemyDeadActions)
        {
            action(deadEnemy);
        }
        activeEnemies.Remove(deadEnemy);
        deadEnemies.Add(deadEnemy);
        deadEnemy.transform.position = GenerateSpawnLocation();
        deadEnemy.gameObject.SetActive(false);
    }
    public static void RegisterEnemySpawnCallback(Action<Enemy> a)
    {
        enemySpawnActions.Add(a);
    }
    public static void RegisterEnemyDeadCallback(Action<Enemy> a)
    {
        enemyDeadActions.Add(a);
    }
}
