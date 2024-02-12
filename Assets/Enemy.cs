using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Enemy : MonoBehaviour
{
    public int maxHealth;
    public int health;
    public float moveSpeed;
    float currentMoveSpeed;
    public Player player;
    Rigidbody2D rb;
    public List<Action<Enemy>> enemyDeadActions = new();
    public float moveSpeedMultiplier = 1f;
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        moveSpeedMultiplier = 1f;
        currentMoveSpeed = moveSpeed;
    }
    void Update()
    {
        if (GameManager.upgradeScreenActive)
        {
            rb.velocity = Vector2.zero;
            return;
        }
        if (player == null)
        {
            return;
        }
        transform.right = player.transform.position - transform.position;
    }
    void FixedUpdate()
    {
        if (GameManager.upgradeScreenActive)
        {
            rb.velocity = Vector2.zero;
            return;
        }
        rb.velocity = transform.right * currentMoveSpeed;
    }
    public void RegisterEnemyDeadCallback(Action<Enemy> a)
    {
        enemyDeadActions.Add(a);
    }
    private void OnParticleCollision(GameObject other)
    {
        Debug.Log("Collides with = " + other.gameObject.tag);
        if(health > 0)
        {
            health--;
        }
        if (health <= 0)
        {
            health = 0;
            currentMoveSpeed = 0;
            rb.velocity = Vector2.zero;
            rb.drag = 1;
            foreach (Action<Enemy> action in enemyDeadActions)
            {
                action(this);
            }
        }
    }
    public void ReviveAtLocation(Vector3 reviveLocation)
    {
        transform.position = reviveLocation;
        health = maxHealth;
        currentMoveSpeed = moveSpeed;
        rb.drag = 0;
    }
}
