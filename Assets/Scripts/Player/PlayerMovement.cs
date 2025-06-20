using System.Collections;
using System.Collections.Generic;
using minyee2913.Utils;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour, Knockbackable
{
    PlayerBattle battle;
    public float moveSpeed;
    public float slowRate, slowDown;
    public CharacterController controller;
    StatController stat;
    Vector3 _moveDirection;
    #region Serialize Values

    [SerializeField]
    float jumpHeight = 2f;
    [SerializeField]
    float rollPower, rollTime;
    public bool isJumping;
    [SerializeField]
    private float gravityMultiply = 1f;
    private Vector3 _velocity;
    [SerializeField]
    private Vector3 groundCheck;
    [SerializeField]
    private float groundDistance;
    [SerializeField]
    private LayerMask groundLayer;
    public bool inRoll;
    #endregion

    public Cooldown rollCool = new(3);

    private const float Gravity = -9.81f;
    void Awake()
    {
        controller = GetComponent<CharacterController>();
        stat = GetComponent<StatController>();
        battle = GetComponent<PlayerBattle>();
    }

    public void MoveByInput(Vector2 moveDelta) {
        _moveDirection = transform.forward * moveDelta.y + transform.right * moveDelta.x;
        _moveDirection *= moveSpeed;

        if (slowDown > 0)
        {
            _moveDirection *= slowRate;
        }

        controller.Move(_moveDirection * Time.deltaTime);
    }

    public void JumpByInput() {
        if (!IsGround()) return;

        Jump();
    }

    public bool Roll(Vector2 moveDelta)
    {
        if (rollCool.IsIn())
            return false;

        rollCool.Start();

        StartCoroutine(roll(moveDelta));

        return true;
    }

    IEnumerator roll(Vector2 moveDelta)
    {
        Vector3 delta = transform.forward * moveDelta.y + transform.right * moveDelta.x;

        inRoll = true;

        SetVelocity(delta * rollPower);

        yield return new WaitForSeconds(rollTime);

        SetVelocity(Vector3.zero);

        inRoll = false;
    }

    public void SetVelocity(Vector3 velocity)
    {
        _velocity = velocity;
    }

    public void Teleport(Vector3 pos)
    {
        controller.enabled = false;
        transform.position = pos;
        controller.enabled = true;
    }

    public void Jump()
    {
        _velocity.y = Mathf.Sqrt(jumpHeight * -2f * GetGravity());

        isJumping = true;
    }

    public void ImplementGravity() {
        if (IsGround() && _velocity.y < 0) {
            _velocity.y = -2f;

            if (isJumping)
            {
                isJumping = false;

                battle.JumpKnockback();
            }
        }

        if (slowDown > 0)
        {
            slowDown -= Time.deltaTime;
        }

        _velocity.y += GetGravity() * Time.deltaTime;
        controller.Move(_velocity * Time.deltaTime);
    }

    public bool IsGround() {
        return Physics.CheckSphere(groundCheck + transform.position, groundDistance, groundLayer);
    }

    public float GetGravity() {
        return Gravity * gravityMultiply;
    }

    void OnDrawGizmos() {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(groundCheck + transform.position, groundDistance);
    }

    public bool GiveKnockback(float power, float height, int direction)
    {
        StartCoroutine(knockback(power, height, direction * -Vector3.right));

        return true;
    }
    public bool GiveKnockback(float power, float height, Vector3 center)
    {
        StartCoroutine(knockback(power, height, center));

        return true;
    }

    IEnumerator knockback(float power, float height, Vector3 dir)
    {
        _velocity += new Vector3(0, height) + dir * power;

        yield return new WaitForSeconds(0.3f);

        _velocity = Vector3.zero;
    }
}
