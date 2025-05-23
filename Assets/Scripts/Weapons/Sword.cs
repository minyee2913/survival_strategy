using System.Collections;
using System.Collections.Generic;
using minyee2913.Utils;
using UnityEngine;


public class Sword : Weapon
{
    public string Name, Id;
    #region Serializes
    [SerializeField]
    float atkWaitTime, atkAfterTime, atkCool, slowTime, shakePower, shakeTime;
    [SerializeField]
    int atkDamage;
    [SerializeField]
    [Range(0, 1)]
    float slowRate;
    [SerializeField]
    List<string> animationTrigger;
    [SerializeField]
    List<string> ranges;
    [SerializeField]
    LayerMask targetMask;
    #endregion

    #region forSystem
    float cooldown;
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

    public override IEnumerator RightClickDown(PlayerController player)
    {
        cooldown = atkCool;
        player.movement.slowDown = slowTime;
        player.movement.slowRate = slowRate;

        player.animator.Trigger(animationTrigger[0]);

        yield return new WaitForSeconds(atkWaitTime);

        var targets = player.battle.range.GetHitInRange(player.battle.range.GetRange(ranges[0]), targetMask);

        foreach (Transform transform in targets)
        {
            HealthObject hp = transform.GetComponent<HealthObject>();

            if (hp != null)
            {
                hp.GetDamage(atkDamage, player.battle.health);
            }
        }

        if (targets.Count > 0)
        {
            CamEffector.current.Shake(shakePower, shakeTime);
        }

        yield return new WaitForSeconds(atkAfterTime);
    }

    public override IEnumerator RightClickUp(PlayerController player)
    {
        yield return null;
    }
}
