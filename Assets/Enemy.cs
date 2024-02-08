using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public float moveSpeed;
    public Player player;
    Rigidbody2D rb;
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }
    void Update()
    {
        if(player == null)
        {
            return;
        }
        transform.right = player.transform.position - transform.position;
    }
    void FixedUpdate()
    {
        rb.velocity = transform.right * moveSpeed;
    }
}
