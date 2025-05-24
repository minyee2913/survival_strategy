using System.Collections;
using UnityEngine;

public class Tomb : MonoBehaviour
{
    public Weapon weapon;
    public float range;
    [SerializeField]
    Transform tombItem;
    GameObject weaponModel;

    bool pickingUp;

    // Update is called once per frame
    void Update()
    {
        if (weapon != null)
        {
            if (Vector3.Distance(PlayerController.Local.transform.position, transform.position) <= range && !pickingUp)
            {
                Interaction.Instance.Set(OnInteract, "시체를 뒤적이기");
            }
            else
            {
                Interaction.Instance.UnSet(OnInteract);
            }

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

        pickingUp = false;

        yield return new WaitForSeconds(0.1f);

        if (player.equippment.weapon != null)
        {
            weapon = player.equippment.weapon;
        }

        player.equippment.Equip(wep);
    }
}
