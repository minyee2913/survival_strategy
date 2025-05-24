using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerInput))]
[RequireComponent(typeof(PlayerMovement))]
public class PlayerController : MonoBehaviour
{
    public static PlayerController Local { get; private set; }
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
    [HideInInspector]
    public PlayerEquippment equippment;

    public string state = "idle";

    void Awake()
    {
        input = GetComponent<PlayerInput>();
        movement = GetComponent<PlayerMovement>();
        animator = GetComponent<PlayerAnimator>();
        cam = GetComponent<PlayerCamera>();
        battle = GetComponent<PlayerBattle>();
        equippment = GetComponent<PlayerEquippment>();

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        Local = this;
    }

    void Update()
    {
        Vector2 axis = input.GetAxis();
        movement.MoveByInput(axis);
        animator.IsMoving(input.isMoving);

        cam.MouseRotate(input.isMoving);

        if (state == "idle")
        {
            animator.WaitMotion(input.isMoving);
        }

        if (input.GetJumpInput())
        {
            movement.JumpByInput();
            animator.TriggerJump();
            animator.ResetWait();
        }

        animator.SetJump(movement.isJumping);

        animator.SetMove(axis.x, axis.y);

        movement.ImplementGravity();
        equippment.SyncHandOffset();

        //weapons

        if (equippment.weapon != null)
        {
            if (Input.GetMouseButtonDown(0))
            {
                StartCoroutine(equippment.weapon.RightClickDown(this));
                animator.ResetWait();
            }
            if (Input.GetMouseButtonUp(0))
            {
                StartCoroutine(equippment.weapon.RightClickUp(this));
                animator.ResetWait();
            }

            if (Input.GetMouseButtonDown(1))
            {
                StartCoroutine(equippment.weapon.LeftClickDown(this));
                animator.ResetWait();
            }
            if (Input.GetMouseButtonUp(1))
            {
                StartCoroutine(equippment.weapon.LeftClickUp(this));
                animator.ResetWait();
            }

            StartCoroutine(equippment.weapon.WeaponUpdate(this));
        }
    }
}
