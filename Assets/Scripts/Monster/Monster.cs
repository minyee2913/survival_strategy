using minyee2913.Utils;
using UnityEngine;

public class Monster : MonoBehaviour {
    public HealthObject health;
    public Animator animator;

    void Awake()
    {
        health = GetComponent<HealthObject>();
        animator = GetComponent<Animator>();
    }
}