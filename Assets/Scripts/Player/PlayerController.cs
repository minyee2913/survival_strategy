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
    PlayerCamera cam;
    void Awake()
    {
        input = GetComponent<PlayerInput>();
        movement = GetComponent<PlayerMovement>();
        animator = GetComponent<PlayerAnimator>();
        cam = GetComponent<PlayerCamera>();
        
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        Vector2 axis = input.GetAxis();
        movement.MoveByInput(axis);
        animator.IsMoving(input.isMoving);

        cam.MouseRotate(input.isMoving);
        animator.WaitMotion(input.isMoving);

        if (input.GetJumpInput()) {
            movement.JumpByInput();
            animator.TriggerJump();
        }

        animator.SetJump(movement.isJumping);

        animator.SetMove(axis.x, axis.y);

        movement.ImplementGravity();
    }
}
