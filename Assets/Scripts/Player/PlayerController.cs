using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerInput))]
[RequireComponent(typeof(PlayerMovement))]
public class PlayerController : MonoBehaviour
{
    PlayerInput input;
    PlayerMovement movement;
    PlayerAnimator animator;
    void Awake()
    {
        input = GetComponent<PlayerInput>();
        movement = GetComponent<PlayerMovement>();
        animator = GetComponent<PlayerAnimator>();
        
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        Vector2 axis = input.GetAxis();
        movement.MoveByInput(axis);

        if (input.GetJumpInput())
            movement.JumpByInput();

        animator.SetMove(axis.x, axis.y);

        movement.ImplementGravity();
    }
}
