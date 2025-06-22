using minyee2913.Utils;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float speed = 20f;
    public float lifeTime = 5f;
    public int Damage;
    public GameObject hitEffect;
    public HealthObject attacker;
    public GameObject child;

    void Start()
    {
        // 카메라 정면 방향을 기준으로 투사체 발사
        Vector3 shootDirection = Camera.main.transform.forward;
        GetComponent<Rigidbody>().linearVelocity = shootDirection.normalized * speed;

        Destroy(gameObject, lifeTime);
    }

    void Update()
    {
        if (child != null)
            child.transform.Rotate(0, 0, -720 * Time.deltaTime);
    }

    void OnCollisionEnter(Collision other)
    {
        if (other.transform.CompareTag("Player")) return;
        if (other.transform.CompareTag("Projectile")) return;

        Monster mon = other.transform.GetComponent<Monster>();

        if (mon != null)
        {
            mon.health.GetDamage(Damage, attacker, HealthObject.Cause.Range);
            SoundManager.Instance.PlaySound("Effect/shootHurt", 1, 0.4f, 1f, false);

            if (hitEffect != null)
            {
                Instantiate(hitEffect, transform.position, Quaternion.identity);
            }
        }

        Destroy(gameObject);
    }
}
