using System.Collections;
using System.Collections.Generic;
using minyee2913.Utils;
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

        Application.targetFrameRate = 120;


        Local = this;
    }

    void Start()
    {
        battle.health.OnDamage(OnDamage);
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

        if (input.GetRollInput())
        {
            Vector2 rollAxis = axis;
            if (!input.isMoving)
            {
                rollAxis = -Vector2.up;
            }

            if (movement.Roll(rollAxis))
            {
                animator.TriggerRoll();
            }
        }

        animator.SetJump(movement.isJumping);

        animator.SetMove(axis.x, axis.y);

        movement.ImplementGravity();
        equippment.SyncHandOffset();

        animator.SetDeath(battle.health.isDeath);

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

    void OnDamage(HealthObject.OnDamageEv ev)
    {
        if (!battle.health.isDeath)
        {
            animator.TriggerHurt();
        }
    }
}
