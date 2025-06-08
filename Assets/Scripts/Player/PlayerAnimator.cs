using UnityEngine;
using VRM;

public class PlayerAnimator : MonoBehaviour
{
    [SerializeField]
    Animator animator;
    public float wait;
    public bool waiting;
    float lerpDirX, lerpDirY;
    public VRMBlendShapeProxy blendShape;
    void Awake()
    {
        blendShape = GetComponent<VRMBlendShapeProxy>();

        var smrList = GetComponentsInChildren<SkinnedMeshRenderer>();
        foreach (var smr in smrList)
        {
            smr.updateWhenOffscreen = true;
            smr.localBounds = new Bounds(Vector3.zero, Vector3.one * 5f);
        }
    }

    public void WaitMotion(bool isMoving)
    {
        if (isMoving)
        {
            wait = 0;
            waiting = false;
        }
        else
        {
            wait += Time.deltaTime;

            if (wait > 1)
            {
                waiting = true;
            }

            if (wait >= 6)
            {
                wait = -20;

                int waitType = Random.Range(1, 4);

                animator.SetInteger("wait", waitType);
                animator.SetTrigger("waitAction");
            }
        }
    }

    public void ResetWait()
    {
        wait = 0;
    }

    public void SetMove(float x, float y)
    {
        lerpDirX = Mathf.Lerp(lerpDirX, x, 8 * Time.deltaTime);
        lerpDirY = Mathf.Lerp(lerpDirY, y, 8 * Time.deltaTime);
        animator.SetFloat("dirX", lerpDirX);
        animator.SetFloat("dirY", lerpDirY);
    }

    public void IsMoving(bool val)
    {
        animator.SetBool("isMoving", val);
    }

    public void SetJump(bool val)
    {
        animator.SetBool("isJumping", val);
    }

    public void TriggerJump()
    {
        animator.SetTrigger("jump");
    }

    public void TriggerRoll()
    {
        animator.SetTrigger("roll");
    }

    public void TriggerHurt()
    {
        animator.SetTrigger("hurt");
    }

    public void SetDeath(bool death)
    {
        animator.SetBool("isDeath", death);
    }

    public void Trigger(string id)
    {
        animator.Play(id);
    }

    public void CutMotion(int id)
    {
        animator.SetInteger("cutMotion", id);
        animator.SetTrigger("nextCut");
    }
}
