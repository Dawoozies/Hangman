using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BouncingBullet : MonoBehaviour
{
    public float moveSpeed;
    public Vector2 velocity;
    public Vector2 startVelocity;
    Rigidbody2D rb;
    Vector2 lastVelocity;
    void Start()
    {
        ResetBullet(Vector2.zero);
    }
    void Update()
    {
        if (rb == null)
            rb = GetComponent<Rigidbody2D>();
        if(rb.velocity.magnitude > 0)
            transform.up = rb.velocity;
        if(rb.velocity.magnitude < startVelocity.magnitude)
        {
            rb.velocity = Vector2.ClampMagnitude(rb.velocity*100000f, startVelocity.magnitude);
        }
    }
    public void ResetBullet(Vector2 spawnPosition)
    {
        if(rb == null)
            rb = GetComponent<Rigidbody2D>();
        transform.position = spawnPosition;
        Vector2 closestEnemyPos = EnemySpawnManager.GetClosestEnemyPosition(spawnPosition);
        //Vector2 bulletDir = closestEnemyPos - spawnPosition;
        Vector2 bulletDir = startVelocity;
        transform.up = bulletDir;
        rb.velocity = bulletDir * moveSpeed;
    }
    void OnCollisionEnter2D(Collision2D col)
    {

    }
}
