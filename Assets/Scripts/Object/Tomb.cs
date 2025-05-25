using System.Collections;
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
                if (weapon != null)
                {
                    Interaction.Instance.Set(OnInteract, weapon.Name);
                }
                else
                {
                    Interaction.Instance.Set(OnInteract, "고인에게 돌려주기");
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
        pickingUp = true;

        player.movement.slowDown = 2f;
        player.movement.slowRate = 0;

        player.animator.Trigger("Gathering");

        yield return new WaitForSeconds(0.3f);

        Weapon wep = weapon;
        weapon = null;

        yield return new WaitForSeconds(0.1f);

        if (player.equippment.weapon != null)
        {
            weapon = player.equippment.weapon;
        }

        player.equippment.Equip(wep);

        yield return new WaitForSeconds(1.6f);

        pickingUp = false;
    }
}
