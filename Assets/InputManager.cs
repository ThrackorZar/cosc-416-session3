using System;
using UnityEngine;
using UnityEngine.Events;

public class InputManager : MonoBehaviour
{
    public UnityEvent<Vector2> OnMove = new UnityEvent<Vector2>();
    public UnityEvent OnSpacePressed = new UnityEvent();
    public UnityEvent OnResetPressed = new UnityEvent();

    void Start()
    {
        
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log("Space key pressed");
            OnSpacePressed?.Invoke();
        }

        Vector2 input = Vector2.zero;
        // Horizontal input (A/D)
        if (Input.GetKey(KeyCode.A))
        {
            input.x -= 1;
        }
        if (Input.GetKey(KeyCode.D))
        {
            input.x += 1;
        }
        
        // Vertical input (W/S)
        if (Input.GetKey(KeyCode.W))
        {
            input.y += 1;
        }
        if (Input.GetKey(KeyCode.S))
        {
            input.y -= 1;
        }
        
        // Normalize input if it exceeds magnitude of 1
        if (input.sqrMagnitude > 1f)
        {
            input.Normalize();
        }
        
        OnMove?.Invoke(input);

        if (Input.GetKeyDown(KeyCode.R))
        {
            OnResetPressed?.Invoke();
        }
    }
}
