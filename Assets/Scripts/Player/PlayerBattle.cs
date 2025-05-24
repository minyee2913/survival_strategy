
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
}