using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed;
    [HideInInspector]
    public float slowRate, slowDown;
    CharacterController controller;
    Vector3 _moveDirection;
    #region Serialize Values

    [SerializeField]
    float jumpHeight = 2f;
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
    #endregion

    private const float Gravity = -9.81f;
    void Awake()
    {
        controller = GetComponent<CharacterController>();
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

    public void Jump() {
        _velocity.y = Mathf.Sqrt(jumpHeight * -2f * GetGravity());

        isJumping = true;
    }

    public void ImplementGravity() {
        if (IsGround() && _velocity.y < 0) {
            _velocity.y = -2f;

            if (isJumping) {
                isJumping = false;
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
}
