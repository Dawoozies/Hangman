using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public float moveSpeed;
    void Start()
    {
        InputManager.RegisterMouseInputCallback(MouseInputHandler);
        InputManager.RegisterMoveInputCallback(MoveInputHandler);
    }
    public void MouseInputHandler(Vector2 mouseWorldPosition)
    {
        transform.right = (Vector3)mouseWorldPosition - transform.position;
    }
    public void MoveInputHandler(Vector2 moveInput)
    {
        
    }
}
