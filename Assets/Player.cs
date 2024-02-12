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
    public int maxHealth = 3;
    int health = 3;
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
    public float maxFireRate = 10f;
    public float fireRateBuildUpSpeed = 2f;
    List<Action<int>> playerLostHealthActions = new();
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
    private void OnCollisionStay2D(Collision2D collision)
    {
        if(collision.gameObject.tag == "Enemy")
        {
            health--;
        }
    }
    public void RegisterPlayerLostHealthCallback(Action<int> a)
    {
        playerLostHealthActions.Add(a);
    }
}
