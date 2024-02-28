using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BouncingBullet : MonoBehaviour
{
    public float moveSpeed;
    public Vector2 velocity;
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
    }
    public void ResetBullet(Vector2 spawnPosition)
    {
        if(rb == null)
            rb = GetComponent<Rigidbody2D>();
        transform.position = spawnPosition;
        Vector2 closestEnemyPos = EnemySpawnManager.GetClosestEnemyPosition(spawnPosition);
        Vector2 bulletDir = closestEnemyPos - spawnPosition;
        transform.up = bulletDir;
        rb.velocity = bulletDir * moveSpeed;
    }
    void OnCollisionEnter2D(Collision2D col)
    {
        ContactPoint2D contact = col.GetContact(0);
        Vector2 reflectedVelocity = Vector2.Reflect(rb.velocity, contact.normal);
        Debug.Log("Reflected velocity = " + reflectedVelocity);
        rb.velocity = reflectedVelocity * moveSpeed;
    }
}
