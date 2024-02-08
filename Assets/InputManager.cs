using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    public static List<Action<Vector2>> mouseInputActions = new();
    public static List<Action<Vector2>> moveInputActions = new(); 
    void Update()
    {
        Vector2 mouseScreenPos = Input.mousePosition;
        Vector2 mouseWorldPos = Camera.main.ScreenToWorldPoint(mouseScreenPos);
        foreach (Action<Vector2> action in mouseInputActions)
        {
            action(mouseWorldPos);
        }
        Vector2 moveInput = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        foreach (Action<Vector2> action in moveInputActions)
        {
            action(moveInput);
        }
    }
    public static void RegisterMouseInputCallback(Action<Vector2> action)
    {
        mouseInputActions.Add(action);
    }
    public static void RegisterMoveInputCallback(Action<Vector2> action)
    {
        moveInputActions.Add(action);
    }
}
