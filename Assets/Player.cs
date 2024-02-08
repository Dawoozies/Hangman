using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Player : MonoBehaviour
{
    public float moveSpeed;
    Rigidbody2D rb;
    Camera mainCamera;
    public SpriteRenderer mainTriangle;
    void Start()
    {
        InputManager.RegisterMouseInputCallback(MouseInputHandler);
        InputManager.RegisterMoveInputCallback(MoveInputHandler);
        InputManager.RegisterMouseLeftClickHandler(MouseLeftClickHandler);
        rb = GetComponent<Rigidbody2D>();
        mainTriangle = GetComponentInChildren<SpriteRenderer>();
        mainCamera = Camera.main;
    }
    public void MouseInputHandler(Vector2 mouseWorldPosition)
    {
        transform.right = (Vector3)mouseWorldPosition - transform.position;
    }
    public void MoveInputHandler(Vector2 moveInput)
    {
        rb.velocity = moveInput*moveSpeed;
    }
    public void MouseLeftClickHandler(float heldTime)
    {
        mainTriangle.transform.Rotate(new Vector3(0f, 0f, -45f)*Mathf.Pow(3f+heldTime, 2f)*Time.deltaTime);
        if(heldTime == 0)
        {
            mainTriangle.transform.rotation = Quaternion.AngleAxis(90f, Vector3.forward);
        }
    }
    void FixedUpdate()
    {
        float cameraWidth = mainCamera.pixelWidth;
        float cameraHeight = mainCamera.pixelHeight;
    }
}
