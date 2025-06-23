using System;
using System.Collections;
using minyee2913.Utils;
using UnityEngine;

public class Tomb : MonoBehaviour
{
    [SerializeField]
    WeaponSet weaponSet;
    public Weapon weapon;
    public float range;
    [SerializeField]
    Transform tombItem;
    GameObject weaponModel;

    bool pickingUp;

    void Start()
    {
        if (weapon == null)
        {
            weapon = weaponSet.GetRandomWeapon();
        }    
    }

    void Update()
    {
        if (PlayerController.Local != null)
        {
            if (Vector3.Distance(PlayerController.Local.transform.position, transform.position) <= range && !pickingUp)
            {
                if (!GameManager.ExistNow || GameManager.Instance.day >= 2)
                {
                    if (weapon != null)
                    {
                        Interaction.Instance.Set(OnInteract, weapon.Name);
                    }
                    else if (PlayerController.Local.equippment.weapon != null)
                    {
                        Interaction.Instance.Set(OnInteract, "고인에게 돌려주기");
                    }
                    else
                    {
                        Interaction.Instance.UnSet(OnInteract);
                    }
                }
                else
                {
                    Interaction.Instance.Set(OnInteract, "널브러진 해골");
                }
            }
            else
            {
                Interaction.Instance.UnSet(OnInteract);
            }
        }

        if (weapon != null)
        {
            if (weaponModel == null)
            {
                weaponModel = Instantiate(weapon.modelPrefab);
                weaponModel.transform.SetParent(tombItem);

                weaponModel.transform.localRotation = Quaternion.Euler(0, 0, 0);
                weaponModel.transform.localPosition = weapon.modelOffset;
                weaponModel.transform.localScale = Vector3.one;
            }
        }
        else if (weaponModel != null)
        {
            Destroy(weaponModel.gameObject);

            weaponModel = null;
        }
    }

    void OnInteract(PlayerController player)
    {
        StartCoroutine(PickUp(player));
    }

    IEnumerator PickUp(PlayerController player)
    {
        if (GameManager.ExistNow && GameManager.Instance.day <= 1)
        {
            SoundManager.Instance.PlaySound("Effect/no", 2, 0.3f, 1f, false);

            GameManager.Instance.message.text = "으악... 이게 뭐야...";

            yield return new WaitForSeconds(1);

            GameManager.Instance.message.text = "";

            yield break;
        }
        pickingUp = true;

        float quick = 1 - player.battle.stat.GetResultValue("quickness") * 0.01f;

        player.movement.slowDown = 2f * quick;
        player.movement.slowRate = 0;

        player.animator.Trigger("Gathering");

        if (player.lastTomb != transform)
        {
            player.battle.stat.AddBuf("quick" + DateTime.Now.Ticks, new Buf
            {
                key = "quickness",
                mathType = StatMathType.Add,
                value = 1
            });

            float result = player.battle.stat.Calc("quickness");

            if (result >= 1f)
            {
                if (!UIManager.Instance.tipedQuickness)
                {
                    UIManager.Instance.tip.Open();
                    UIManager.Instance.tip.quickness.SetActive(true);

                    UIManager.Instance.tipedQuickness = true;
                }
            }

            player.lastTomb = transform;
        }

        yield return new WaitForSeconds(0.3f * quick);

        Weapon wep = weapon;
        weapon = null;

        yield return new WaitForSeconds(0.1f * quick);

        if (player.equippment.weapon != null)
        {
            weapon = player.equippment.weapon;
        }

        player.equippment.Equip(wep);

        yield return new WaitForSeconds(1.6f * quick);

        pickingUp = false;
    }
}
