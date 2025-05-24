using minyee2913.Utils;
using UnityEngine;

public class Ifrit : Monster
{
    void Start()
    {
        health.OnDamage(OnDamage);
    }

    void OnDamage(HealthObject.OnDamageEv ev)
    {
        animator.SetTrigger("hurt");
        Debug.Log("hurt");
    }
}
