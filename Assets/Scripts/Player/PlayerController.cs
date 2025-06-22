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
    public Transform lastTomb;

    public string state = "idle";
    public bool NotInControl, charging;
    public float chargeTime;

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
        if (NotInControl)
        {
            animator.SetMove(0, 0);
            return;
        }

        animator.PickSpeedMotion();

        if (charging)
        {
            cam.shootCamera.gameObject.SetActive(true);
            movement.slowDown = 0.3f;
            movement.slowRate = 0;
            chargeTime += Time.deltaTime;
        }
        else
        {
            cam.shootCamera.gameObject.SetActive(false);
            chargeTime = 0;
        }

        Vector2 axis = input.GetAxis();

        if (!battle.health.isDeath)
        {
            movement.MoveByInput(axis);
            animator.IsMoving(input.isMoving);
        }
        else
        {
            animator.IsMoving(false);
        }

        if (!battle.health.isDeath)
            cam.MouseRotate(!animator.waiting);

        if (state == "idle" && movement.slowDown <= 0)
            {
                animator.WaitMotion(input.isMoving);
            }

        if (input.GetJumpInput() && !battle.health.isDeath)
        {
            movement.JumpByInput();
            animator.TriggerJump();
            animator.ResetWait();
        }

        if (input.GetRollInput() && !battle.health.isDeath)
        {
            Vector2 rollAxis = axis;
            if (!input.isMoving)
            {
                rollAxis = -Vector2.up;
            }

            if (movement.Roll(rollAxis))
            {
                animator.Trigger("Roll Tree");
            }
        }

        animator.SetJump(movement.isJumping);

        animator.SetMove(axis.x, axis.y);

        movement.ImplementGravity();
        equippment.SyncHandOffset();

        animator.SetDeath(battle.health.isDeath);

        //weapons

        if (equippment.weapon != null && !battle.health.isDeath)
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
