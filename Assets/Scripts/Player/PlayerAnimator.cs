using UnityEngine;

public class PlayerAnimator : MonoBehaviour
{
    Animator animator;
    public float wait;

    void Awake()
    {
        animator = GetComponent<Animator>();
    }

    public void WaitMotion(bool isMoving) {
        if (isMoving) {
            wait = 0;
        } else {
            wait += Time.deltaTime;

            if (wait >= 6) {
                wait = -20;

                int waitType = Random.Range(1, 4);

                animator.SetInteger("wait", waitType);
                animator.SetTrigger("waitAction");
            }
        }
    }

    public void SetMove(float x, float y) {
        animator.SetFloat("dirX", x);
        animator.SetFloat("dirY", y);
    }

    public void IsMoving(bool val) {
        animator.SetBool("isMoving", val);
    }

    public void SetJump(bool val) {
        animator.SetBool("isJumping", val);
    }

        public void TriggerJump() {
        animator.SetTrigger("jump");
    }
}
