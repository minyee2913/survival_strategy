using System.Collections.Generic;
using minyee2913.Utils;
using UnityEngine;

public class Ifrit : Monster
{
    public string state;
    [SerializeField]
    float tick, lastLoopCount;
    string actionData;
    bool halfTriggered;
    void Start()
    {
        health.OnDamage(OnDamage);

        state = "idle";
    }

    void OnDamage(HealthObject.OnDamageEv ev)
    {
        animator.SetTrigger("hurt");
    }

    void FixedUpdate()
    {
        tick += Time.fixedDeltaTime;

        switch (state)
        {
            case "idle":
                Idle();
                break;
            case "follow":
                Follow();
                break;
        }


        AnimateMove();
    }

    void Idle()
    {
        if (actionData != "idleMotion")
        {
            animator.Play("action1");
            actionData = "idleMotion";
        }

        if (tick > 3 && TargetIsIn(30))
        {
            tick = 0;
            state = "follow";
            actionData = "";
        }

        if (tick >= 4)
        {
            tick = 0;
            actionData = "";
        }
    }

    void Follow()
    {
        agent.stoppingDistance = 0;
        Chase(20);

        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);

        if (IsMoving())
        {
            float normalizedTime = stateInfo.normalizedTime;
            int loopCount = Mathf.FloorToInt(normalizedTime);
            float fractional = normalizedTime % 1f;

            if (loopCount > lastLoopCount)
            {
                lastLoopCount = loopCount;
                halfTriggered = false;
                OnWalkCycleEnd("half");
            }

            if (!halfTriggered && fractional >= 0.5f && fractional < 0.75f) // 안전 범위 추가
            {
                halfTriggered = true;
                OnWalkCycleEnd("end");
            }
        }
        else
        {
            halfTriggered = false;
            lastLoopCount = 0;
        }

        if (tick >= 5 && TargetIsIn(20))
        {
            Attack();
            tick = -Random.Range(1, 3);
        }
    }


    void OnWalkCycleEnd(string when)
    {
        CamEffector.current.Shake(4, 0.3f);

        List<Transform> targets;

        if (when == "half")
        {
            targets = range.GetHitInRange(range.GetRange("footLeft"), LayerMask.GetMask("player"));
        }
        else
        {
            targets = range.GetHitInRange(range.GetRange("footRight"), LayerMask.GetMask("player"));
        }

        float damage = stat.GetResult()["attackDamage"] * 1.2f;

        foreach (Transform target in targets)
        {
            HealthObject hp = target.GetComponent<HealthObject>();
            Knockbackable kn = target.GetComponent<Knockbackable>();

            if (kn != null)
            {
                kn.GiveKnockback(8, 10, target.transform.position - transform.position);
            }

            if (hp != null)
            {
                hp.GetDamage((int)damage, health);
            }
        }
    }

    void Attack()
    {
        animator.Play("Attack");
        stopMove = 2f;

        float damage = stat.GetResult()["attackDamage"] * 2f;

        foreach (Transform target in range.GetHitInRange(range.GetRange("kick"), LayerMask.GetMask("player")))
        {
            HealthObject hp = target.GetComponent<HealthObject>();
            Knockbackable kn = target.GetComponent<Knockbackable>();

            if (kn != null)
            {
                kn.GiveKnockback(5, 30, target.transform.position - transform.position);
            }

            if (hp != null)
            {
                hp.GetDamage((int)damage, health);
            }
        }
    }
}
