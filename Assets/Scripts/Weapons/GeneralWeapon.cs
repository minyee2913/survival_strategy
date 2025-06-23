using System.Collections;
using System.Collections.Generic;
using minyee2913.Utils;
using UnityEngine;

public enum GeneralWeaponDirection
{
    Forward,
    Backward,
    Left,
    Right,
    Up,
    Down,
}

[System.Serializable]
public struct GeneralAtkAfterAtk
{
    public float atkWaitTime;
    public float damageRate;
}

[System.Serializable]
public struct GeneralAtkTypes
{
    [Header("base")]
    public float atkWaitTime;
    public float atkCool, slowTime, shakePower, shakeTime;
    public int damageRate;
    [Range(0, 1)]
    public float slowRate;
    public string animationTrigger, range;
    [Header("dash Effect")]
    public float dashPower;
    public float dashTime;
    public GeneralWeaponDirection dashDirection;
    public bool dashBeforeWait;
    public List<GeneralAtkAfterAtk> afterAtk;
}

[CreateAssetMenu(fileName = "General", menuName = "weapons/General", order = int.MaxValue)]
public class GeneralWeapon : Weapon
{
    #region Serializes
    [SerializeField]
    List<GeneralAtkTypes> types;
    [SerializeField]
    LayerMask targetMask;
    #endregion

    #region forSystem
    float cooldown;
    int atkType;
    Cooldown shoot = new(1);
    #endregion

    public override IEnumerator WeaponUpdate(PlayerController player)
    {
        if (cooldown > 0)
        {
            cooldown -= Time.deltaTime;
        }

        if (player.charging && player.movement.inRoll)
        {
            player.charging = false;
            player.animator.Trigger("shoot");
            player.battle.chargeVol.weight = 0;
        }

        yield break;
    }

    public override IEnumerator LeftClickDown(PlayerController player)
    {
        if (shoot.IsIn())
            yield break;
        shoot.Start();
        
        player.charging = true;
        player.animator.Trigger("charge");
        player.battle.chargeVol.weight = 1;

        SoundManager.Instance.PlaySound("Effect/charge", 1, 1f, 1f, false);

        yield break;
    }

    public override IEnumerator LeftClickUp(PlayerController player)
    {
        if (!player.charging)
            yield break;

        float rate = player.chargeTime;
        if (rate > 1.5f)
            rate = 1.5f;

        player.battle.chargeVol.weight = 0;

        player.charging = false;
        player.animator.Trigger("shoot");

        Projectile pro = Instantiate(player.battle.projectile, player.transform.position + new Vector3(0, 2f), Quaternion.identity);
        pro.child = Instantiate(modelPrefab, pro.transform);
        pro.child.transform.localPosition = Vector3.zero;

        pro.Damage = (int)(player.battle.AttackDamage() * rate);
        pro.attacker = player.battle.health;

        SoundManager.Instance.PlaySound("Effect/shoot", 1, 1f, 1f, false);

        player.equippment.Equip(null);

        yield break;
    }

    IEnumerator DashAction(PlayerController player, GeneralAtkTypes typeAtk)
    {
        switch (typeAtk.dashDirection)
        {
            case GeneralWeaponDirection.Forward:
                player.movement.SetVelocity(player.transform.forward * typeAtk.dashPower);
                break;
            case GeneralWeaponDirection.Backward:
                player.movement.SetVelocity(-player.transform.forward * typeAtk.dashPower);
                break;
            case GeneralWeaponDirection.Right:
                player.movement.SetVelocity(player.transform.right * typeAtk.dashPower);
                break;
            case GeneralWeaponDirection.Left:
                player.movement.SetVelocity(-player.transform.right * typeAtk.dashPower);
                break;
            case GeneralWeaponDirection.Up:
                player.movement.SetVelocity(player.transform.up * typeAtk.dashPower);
                break;
            case GeneralWeaponDirection.Down:
                player.movement.SetVelocity(-player.transform.up * typeAtk.dashPower);
                break;
        }

        yield return new WaitForSeconds(typeAtk.dashTime);

        player.movement.SetVelocity(Vector3.zero);
    }

    public override IEnumerator RightClickDown(PlayerController player)
    {
        if (cooldown > 0)
        {
            yield break;
        }

        SoundManager.Instance.PlaySound("Effect/fast_sword", 1, 0.3f, 1f, false);

        GeneralAtkTypes typeAtk = types[atkType];

        cooldown = typeAtk.atkCool;
        player.movement.slowDown = typeAtk.slowTime;
        player.movement.slowRate = typeAtk.slowRate;

        player.animator.Trigger(typeAtk.animationTrigger);

        if (typeAtk.dashBeforeWait && typeAtk.dashTime > 0)
        {
            player.StartCoroutine(DashAction(player, typeAtk));
        }

        yield return new WaitForSeconds(typeAtk.atkWaitTime);

        if (player.movement.inRoll)
            yield break;

        if (!typeAtk.dashBeforeWait && typeAtk.dashTime > 0)
        {
            player.StartCoroutine(DashAction(player, typeAtk));
        }

        var targets = player.battle.range.GetHitInRange(player.battle.range.GetRange(typeAtk.range), targetMask);

        foreach (Transform transform in targets)
        {
            HealthObject hp = transform.GetComponent<HealthObject>();

            if (hp != null)
            {
                float damage = player.battle.AttackDamage() * typeAtk.damageRate * 0.01f;
                hp.GetDamage((int)damage, player.battle.health, HealthObject.Cause.Melee);
            }
        }

        if (targets.Count > 0)
        {
            CamEffector.current.Shake(typeAtk.shakePower, typeAtk.shakeTime);

            SoundManager.Instance.PlaySound("Effect/sword1", 1, 0.3f, 1f, false);

            if (Random.Range(0, 100f) <= 40)
            {
                if (Random.Range(0, 50f) <= 25)
                {
                    SoundManager.Instance.PlaySound("Effect/atk1", 2, 0.3f, 1f, false);
                }
                else
                {
                    SoundManager.Instance.PlaySound("Effect/atk2", 2, 0.3f, 1f, false);
                }
            }
        }

        if (types.Count > 1)
        {
            atkType++;

            if (atkType >= types.Count)
            {
                atkType = 0;
            }
        }

        if (typeAtk.afterAtk != null && typeAtk.afterAtk.Count > 0)
        {
            foreach (GeneralAtkAfterAtk afterAtk in typeAtk.afterAtk)
            {
                yield return new WaitForSeconds(afterAtk.atkWaitTime);

                foreach (Transform transform in targets)
                {
                    HealthObject hp = transform.GetComponent<HealthObject>();

                    if (hp != null)
                    {
                        float damage = player.battle.AttackDamage() * afterAtk.damageRate * 0.01f;
                        hp.GetDamage((int)damage, player.battle.health, HealthObject.Cause.Melee);
                    }
                }
            }
        }
    }

    public override IEnumerator RightClickUp(PlayerController player)
    {
        yield return null;
    }
}
