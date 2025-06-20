
using System;
using minyee2913.Utils;
using UnityEngine;

public class PlayerBattle : MonoBehaviour
{
    public RangeController range;
    public HealthObject health;
    public StatController stat;
    public PlayerMovement movement;
    Transform beforeKick;

    void Awake()
    {
        range = GetComponent<RangeController>();
        health = GetComponent<HealthObject>();
        stat = GetComponent<StatController>();
        movement = GetComponent<PlayerMovement>();

        health.OnGiveDamage(OnGiveDamage);
        health.OnDamage(OnDamage);
        health.OnDamageFinal(OnDamageFinal);
    }

    public float AttackDamage()
    {
        return stat.GetResult()["attackDamage"];
    }

    void OnDamage(HealthObject.OnDamageEv ev)
    {
        if (movement.inRoll)
        {
            ev.cancel = true;
            return;

        }
        ev.Damage -= (int)stat.GetResultValue("defense");
    }

    void OnDamageFinal(HealthObject.OnDamageFinalEv ev)
    {
        IndicatorManager.Instance.GenerateText(ev.Damage.ToString(), transform.position + new Vector3(UnityEngine.Random.Range(-1f, 1f), 1), Color.red);
    }

    void OnGiveDamage(HealthObject.OnGiveDamageEv ev)
    {
        stat.AddBuf("atk" + DateTime.Now.Ticks, new Buf
        {
            key = "attackDamage",
            mathType = StatMathType.Increase,
            value = 25
        });

        stat.Calc("attackDamage");
    }

    public void JumpKnockback()
    {
        var targets = range.GetHitInRange(range.GetRange("jump_pushing"), LayerMask.GetMask("ground"));

        foreach (var target in targets)
        {
            if (!target.CompareTag("pushable"))
            {
                continue;
            }
            Rigidbody rigidbody = target.GetComponent<Rigidbody>();

            if (rigidbody != null)
            {
                rigidbody.linearVelocity = (target.position - transform.position) * 8;

                SoundManager.Instance.PlaySound("Effect/step_skeleton", 2, 0.3f, 1f, false);

                if (beforeKick != target)
                {
                    stat.AddBuf("def" + DateTime.Now.Ticks, new Buf
                    {
                        key = "defense",
                        mathType = StatMathType.Add,
                        value = 0.2f
                    });

                    stat.Calc("defense");

                    beforeKick = target;
                }
            }
        }
    }
}