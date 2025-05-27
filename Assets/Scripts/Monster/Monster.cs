using minyee2913.Utils;
using UnityEngine;
using UnityEngine.AI;

public class Monster : MonoBehaviour
{
    public HealthObject health;
    public Animator animator;
    public NavMeshAgent agent;

    public float stopMove, atkCool;

    void Awake()
    {
        health = GetComponent<HealthObject>();
        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
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

    protected void AnimateMove()
    {
        Vector3 velocity = agent.velocity;

        Vector3 localVelocity = transform.InverseTransformDirection(velocity);

        animator.SetFloat("dirX", localVelocity.x);
        animator.SetFloat("dirY", localVelocity.z);
    }
}