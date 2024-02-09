using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Android;

public class CameraEffects : MonoBehaviour
{
    Vector3 originalPosition;
    Vector3 newPosition;
    Camera mainCamera;
    public float maxCameraShake;
    public float smoothTime;
    Vector3 velocity;
    float enemyDeathCameraShake;
    void Start()
    {
        mainCamera = Camera.main;
        originalPosition = mainCamera.transform.position;
        InputManager.RegisterMouseLeftClickHandler(MouseLeftClickHandler);
        EnemySpawnManager.RegisterEnemySpawnCallback((Enemy enemySpawned) => enemySpawned.RegisterEnemyDeadCallback(EnemyDeadHandler));
    }
    private void Update()
    {
        mainCamera.transform.position = Vector3.SmoothDamp(mainCamera.transform.position, newPosition + CameraShakeVector(enemyDeathCameraShake), ref velocity, smoothTime);
        if(enemyDeathCameraShake > 0f)
        {
            enemyDeathCameraShake -= Time.deltaTime;
            if(enemyDeathCameraShake <= 0f)
            {
                enemyDeathCameraShake = 0f;
            }
        }
    }
    public void MouseLeftClickHandler(float heldTime)
    {
        if (heldTime == 0)
        {
            newPosition = originalPosition;
        }
        else
        {
            float shakeDistance = Mathf.Min(2 * heldTime, maxCameraShake);
            newPosition = CameraShakeVector(shakeDistance);
        }
    }
    Vector3 CameraShakeVector(float shakeDistance)
    {
        Vector3 shakeVector = Vector3.zero;
        float randomX = Mathf.Cos(Random.Range(0, Mathf.PI));
        float randomY = Mathf.Sin(Random.Range(-Mathf.PI / 2, Mathf.PI / 2));
        shakeVector = new Vector3(randomX, randomY, 0) * shakeDistance;
        shakeVector.z = -10;
        return shakeVector;
    }
    void EnemyDeadHandler(Enemy deadEnemy)
    {
        enemyDeathCameraShake = 1f;
    }
}
