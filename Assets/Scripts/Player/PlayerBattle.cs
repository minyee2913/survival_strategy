
using System;
using System.Collections;
using minyee2913.Utils;
using UnityEngine;
using UnityEngine.Rendering;

public class PlayerBattle : MonoBehaviour
{
    public RangeController range;
    public Animator animator;
    public HealthObject health;
    public StatController stat;
    public PlayerMovement movement;
    Transform beforeKick;
    public Volume dodgeVol, chargeVol;
    public Projectile projectile;

    void Awake()
    {
        range = GetComponent<RangeController>();
        health = GetComponent<HealthObject>();
        stat = GetComponent<StatController>();
        movement = GetComponent<PlayerMovement>();
        animator = GetComponent<Animator>();

        health.OnGiveDamage(OnGiveDamage);
        health.OnDamage(OnDamage);
        health.OnDamageFinal(OnDamageFinal);
        health.OnDeath(OnDeath);
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

            StartCoroutine(Dodged());
            return;

        }
        ev.Damage -= (int)stat.GetResultValue("defense");
    }

    IEnumerator Dodged()
    {
        animator.Play("shield");
        CamEffector.current.Shake(5, 0.2f);
        CamEffector.current.ViewUp(-2, 0, 0.2f);

        SoundManager.Instance.PlaySound("Effect/shield", 1, 0.3f, 1f, false);
        SoundManager.Instance.PlaySound("Effect/dodge", 2, 0.3f, 1f, false);

        movement.GiveKnockback(-5, 0, transform.forward);

        Time.timeScale = 0.3f;

        for (int i = 0; i <= 10; i++)
        {
            dodgeVol.weight = i * 0.1f;

            yield return new WaitForSecondsRealtime(0.05f);
        }

        yield return new WaitForSecondsRealtime(0.1f);

        Time.timeScale = 1f;

        for (int i = 10; i > 0; i--)
        {
            dodgeVol.weight = i * 0.1f;

            yield return new WaitForSecondsRealtime(0.01f);
        }

        CamEffector.current.ViewOut(0.3f);

        dodgeVol.weight = 0;
    }

    void OnDamageFinal(HealthObject.OnDamageFinalEv ev)
    {
        IndicatorManager.Instance.GenerateText(ev.Damage.ToString(), transform.position + new Vector3(UnityEngine.Random.Range(-1f, 1f), 1), Color.red);

        SoundManager.Instance.PlaySound("Effect/hurt", 2, 0.3f, 1f, false);
    }

    void OnDeath(HealthObject.OnDamageEv ev)
    {
        SoundManager.Instance.PlaySound("Effect/death", 2, 0.3f, 1f, false);
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

                    float result = stat.Calc("defense");

                    if (result >= 0.5f)
                    {
                        if (!UIManager.Instance.tipedDefense)
                        {
                            UIManager.Instance.tip.Open();
                            UIManager.Instance.tip.defense.SetActive(true);

                            UIManager.Instance.tipedDefense = true;
                        }
                    }

                    beforeKick = target;
                }
            }
        }
    }
}