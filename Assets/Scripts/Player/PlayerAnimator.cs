using UnityEngine;

public class PlayerAnimator : MonoBehaviour
{
    Animator animator;

    void Awake()
    {
        animator = GetComponent<Animator>();
    }

    public void SetMove(float x, float y) {
        animator.SetFloat("dirX", x);
        animator.SetFloat("dirY", y);
    }
}
