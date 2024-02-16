using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BouncingBullet : MonoBehaviour
{
    public float moveSpeed;
    public Vector2 velocity;
    Rigidbody2D rb;
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }
    public void ResetBullet(Vector2 spawnPosition)
    {
        transform.position = spawnPosition;
        Vector2 closestEnemyPos = EnemySpawnManager.GetClosestEnemyPosition(spawnPosition);
        Vector2 bulletDir = closestEnemyPos - spawnPosition;
        transform.up = bulletDir;
        rb.velocity = bulletDir * moveSpeed;
    }
    void OnCollisionEnter2D(Collision2D col)
    {
        if(col.gameObject.tag == "Player")
        {
            return;
        }
        ContactPoint2D contactPoint = col.GetContact(0);
        Vector2 reflectedVelocity = Vector2.Reflect(rb.velocity, contactPoint.normal);
    }
}
