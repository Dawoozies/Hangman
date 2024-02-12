using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class Player : MonoBehaviour
{
    public float moveSpeed;
    Rigidbody2D rb;
    public Transform trianglePivot;
    public SpriteRenderer mainTriangle;
    public ParticleSystem gunParticleSystem;
    ParticleSystem.EmissionModule emission;
    Vector3 mouseLook;
    public Transform mouseTarget;
    public int Health { 
        get { return health; }
        set {
            if(value < health)
            {
                foreach (Action<int> action in playerLostHealthActions)
                {
                    action(value);
                }
            }
            health = value;
        }
    }
    public int maxHealth = 3;
    int health = 3;
    public float iFrameTime = 0.5f;
    float iFrameTimer = 0f;
    public float maxFireRate = 10f;
    public float fireRateBuildUpSpeed = 2f;
    List<Action<int>> playerLostHealthActions = new();
    public void ResetGame()
    {
        maxHealth = 3;
        health = 3;
        iFrameTime = 0.5f;
        iFrameTimer = 0f;
        maxFireRate = 10f;
        fireRateBuildUpSpeed = 2f;
        transform.position = Vector3.zero;
    }
    void Start()
    {
        InputManager.RegisterMouseInputCallback(MouseInputHandler);
        InputManager.RegisterMoveInputCallback(MoveInputHandler);
        InputManager.RegisterMouseLeftClickHandler(MouseLeftClickHandler);
        rb = GetComponent<Rigidbody2D>();
        mainTriangle = GetComponentInChildren<SpriteRenderer>();
        emission = gunParticleSystem.emission;
    }
    public void MouseInputHandler(Vector2 mouseWorldPosition)
    {
        trianglePivot.right = (Vector3)mouseWorldPosition - transform.position;
        mouseTarget.position = mouseWorldPosition;
    }
    public void MoveInputHandler(Vector2 moveInput)
    {
        rb.velocity = moveInput*moveSpeed;
    }
    public void MouseLeftClickHandler(float heldTime)
    {
        if(heldTime == 0)
        {
            mainTriangle.transform.localRotation = Quaternion.AngleAxis(-90f, Vector3.forward);
            emission.rateOverTime = 0;
        }
        else
        {
            mainTriangle.transform.Rotate(new Vector3(0f, 0f, -45f) * Mathf.Pow(3f + heldTime, 2f) * Time.deltaTime, Space.Self);
            emission.rateOverTime = Mathf.Lerp(0f, maxFireRate, heldTime*fireRateBuildUpSpeed);
        }
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.tag == "Enemy")
        {
            DamagePlayer(collision);
        }
    }
    private void OnCollisionStay2D(Collision2D collision)
    {
        if(collision.gameObject.tag == "Enemy")
        {
            DamagePlayer(collision);
        }
    }
    void DamagePlayer(Collision2D collision)
    {
        if(iFrameTimer > 0)
        {
            return;
        }
        Health--;
        iFrameTimer = iFrameTime;
    }
    public void RegisterPlayerLostHealthCallback(Action<int> a)
    {
        playerLostHealthActions.Add(a);
    }
    private void Update()
    {
        if(iFrameTimer > 0)
        {
            iFrameTimer -= Time.deltaTime;
            mainTriangle.color = Color.Lerp(Color.white, Color.clear, Mathf.RoundToInt(Mathf.PingPong(16f*Time.time, 1f)));
        }
        else
        {
            mainTriangle.color = Color.white;
        }
    }
}
