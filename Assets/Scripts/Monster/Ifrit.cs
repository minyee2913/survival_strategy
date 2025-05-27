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

        if (tick > 3 && TargetIsIn(15))
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
        Chase(20);

        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);

        if (stateInfo.IsName("Move Tree"))
        {
            float normalizedTime = stateInfo.normalizedTime;
            int currentLoopCount = Mathf.FloorToInt(normalizedTime);
            float fractional = normalizedTime - currentLoopCount;

            // 루프 완주 이벤트 (1.0, 2.0, 3.0 ...)
            if (currentLoopCount > lastLoopCount)
            {
                lastLoopCount = currentLoopCount;
                halfTriggered = false; // 절반 이벤트 초기화
                OnWalkCycleEnd();          // 완료 이벤트
            }

            // 절반 지점 이벤트 (0.5, 1.5, 2.5 ...)
            if (!halfTriggered && fractional >= 0.5f)
            {
                halfTriggered = true;
                OnWalkCycleEnd();          // 절반 도달 이벤트
            }
        }
    }

    void OnWalkCycleEnd()
    {
        CamEffector.current.Shake(6, 0.3f);
    }
}
