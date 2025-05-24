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
}

[CreateAssetMenu(fileName = "General", menuName = "weapons/General", order = int.MaxValue)]
public class GeneralWeapon : Weapon
{
    public string Name, Id;
    #region Serializes
    [SerializeField]
    List<GeneralAtkTypes> types;
    [SerializeField]
    LayerMask targetMask;
    #endregion

    #region forSystem
    float cooldown;
    int atkType;
    #endregion

    public override IEnumerator WeaponUpdate(PlayerController player)
    {
        if (cooldown > 0)
        {
            cooldown -= Time.deltaTime;
        }

        yield break;
    }

    public override IEnumerator LeftClickDown(PlayerController player)
    {
        yield return null;
    }

    public override IEnumerator LeftClickUp(PlayerController player)
    {
        yield return null;
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
                hp.GetDamage((int)damage, player.battle.health);
            }
        }

        if (targets.Count > 0)
        {
            CamEffector.current.Shake(typeAtk.shakePower, typeAtk.shakeTime);
        }

        if (types.Count > 1)
        {
            atkType++;

            if (atkType >= types.Count)
            {
                atkType = 0;
            }
        }
    }

    public override IEnumerator RightClickUp(PlayerController player)
    {
        yield return null;
    }
}
