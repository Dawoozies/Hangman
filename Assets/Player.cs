using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

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
            emission.rateOverTime = Mathf.Lerp(0f, 10f, heldTime*4);
        }
    }

}
