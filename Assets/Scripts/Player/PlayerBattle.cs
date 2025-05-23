
using minyee2913.Utils;
using UnityEngine;

public class PlayerBattle : MonoBehaviour
{
    public RangeController range;
    public HealthObject health;

    public Weapon weapon;

    void Awake()
    {
        range = GetComponent<RangeController>();
        health = GetComponent<HealthObject>();
    }

}