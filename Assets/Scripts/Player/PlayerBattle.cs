
using minyee2913.Utils;
using UnityEngine;

public class PlayerBattle : MonoBehaviour
{
    public RangeController range;
    public HealthObject health;
    public StatController stat;

    void Awake()
    {
        range = GetComponent<RangeController>();
        health = GetComponent<HealthObject>();
        stat = GetComponent<StatController>();
    }

    public float AttackDamage()
    {
        return stat.GetResult()["attackDamage"];
    }

    public void JumpKnockback()
    {
        var targets = range.GetHitInRange(range.GetRange("jump_pushing"), LayerMask.GetMask("ground"));

        foreach (var target in targets)
        {
            if (target.tag != "pushable")
            {
                continue;
            }
            Rigidbody rigidbody = target.GetComponent<Rigidbody>();

            if (rigidbody != null)
            {
                rigidbody.linearVelocity = (target.position - transform.position) * 8;

                SoundManager.Instance.PlaySound("Effect/step_skeleton", 2, 0.3f, 1f, false);
            }
        }

        Debug.Log(targets.Count);
    }
}