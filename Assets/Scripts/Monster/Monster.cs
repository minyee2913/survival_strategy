using System.Collections;
using minyee2913.Utils;
using UnityEngine;
using UnityEngine.AI;

public class Monster : MonoBehaviour, Knockbackable
{
    public HealthObject health;
    public Animator animator;
    public NavMeshAgent agent;

    public float stopMove, atkCool;
    public RangeController range;
    public StatController stat;
    public Rigidbody rigid;

    void Awake()
    {
        health = GetComponent<HealthObject>();
        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        range = GetComponent<RangeController>();
        stat = GetComponent<StatController>();
        rigid = GetComponent<Rigidbody>();

        health.OnDamageFinal(onDamageFinal);
    }

    void onDamageFinal(HealthObject.OnDamageFinalEv ev)
    {
        IndicatorManager.Instance.GenerateText(ev.Damage.ToString(), ev.attacker.transform.position + ev.attacker.transform.forward * 1.5f + new Vector3(Random.Range(-1f, 1f), 1), Color.white);
    }

    void Update()
    {
        if (stopMove > 0)
            stopMove -= Time.deltaTime;

        if (atkCool > 0)
            atkCool -= Time.deltaTime;
    }

    protected bool Chase(float distance)
    {
        if (stopMove > 0)
        {
            agent.SetDestination(transform.position);
            return false;
        }

        if (TargetIsIn(distance))
        {
            agent.SetDestination(PlayerController.Local.transform.position);

            return true;
        }

        return false;
    }

    protected bool TargetIsIn(float distance)
    {
        return Vector3.Distance(PlayerController.Local.transform.position, transform.position) <= distance;
    }

    public bool IsMoving()
    {
        Vector3 velocity = agent.velocity;

        Vector3 localVelocity = transform.InverseTransformDirection(velocity);

        return Mathf.Abs(localVelocity.x) + Mathf.Abs(localVelocity.z) != 0;
    }

    protected void AnimateMove()
    {
        Vector3 velocity = agent.velocity;

        Vector3 localVelocity = transform.InverseTransformDirection(velocity);

        animator.SetFloat("dirX", localVelocity.x);
        animator.SetFloat("dirY", localVelocity.z);
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
        rigid.linearVelocity += new Vector3(0, height) + dir * power;

        yield return new WaitForSeconds(0.3f);

        rigid.linearVelocity = Vector3.zero;
    }
}