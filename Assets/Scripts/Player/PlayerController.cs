using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerInput))]
[RequireComponent(typeof(PlayerMovement))]
public class PlayerController : MonoBehaviour
{
    [HideInInspector]
    public PlayerInput input;
    [HideInInspector]
    public PlayerMovement movement;
    [HideInInspector]
    public PlayerAnimator animator;
    [HideInInspector]
    public PlayerCamera cam;
    [HideInInspector]
    public PlayerBattle battle;
    void Awake()
    {
        input = GetComponent<PlayerInput>();
        movement = GetComponent<PlayerMovement>();
        animator = GetComponent<PlayerAnimator>();
        cam = GetComponent<PlayerCamera>();
        battle = GetComponent<PlayerBattle>();
        
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

        if (input.GetJumpInput())
        {
            movement.JumpByInput();
            animator.TriggerJump();
        }

        animator.SetJump(movement.isJumping);

        animator.SetMove(axis.x, axis.y);

        movement.ImplementGravity();

        //weapons

        if (battle.weapon != null)
        {
            if (Input.GetMouseButtonDown(0))
            {
                StartCoroutine(battle.weapon.RightClickDown(this));
            }
            if (Input.GetMouseButtonUp(0))
            {
                StartCoroutine(battle.weapon.RightClickUp(this));
            }

            if (Input.GetMouseButtonDown(1))
            {
                StartCoroutine(battle.weapon.LeftClickDown(this));
            }
            if (Input.GetMouseButtonUp(1))
            {
                StartCoroutine(battle.weapon.LeftClickUp(this));
            }

            StartCoroutine(battle.weapon.WeaponUpdate(this));
        }
    }
}
