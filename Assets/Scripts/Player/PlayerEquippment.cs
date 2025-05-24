using minyee2913.Utils;
using UnityEngine;

public class PlayerEquippment : MonoBehaviour
{
    public StatController stat;
    public Weapon weapon;

    [SerializeField]
    Transform rightItem;
    GameObject weaponModel;

    void Awake()
    {
        stat = GetComponent<StatController>();
    }

    public void SyncHandOffset()
    {
        if (weaponModel != null)
        {
            weaponModel.transform.localPosition = weapon.modelOffset;
        }
    }

    public void Equip(Weapon item)
    {
        weapon = item;

        if (weaponModel != null)
        {
            Destroy(weaponModel.gameObject);

            weaponModel = null;
        }

        if (weapon != null)
        {
            weaponModel = Instantiate(item.modelPrefab);
            weaponModel.transform.SetParent(rightItem);

            weaponModel.transform.localRotation = Quaternion.Euler(0, 0, 0);
            weaponModel.transform.localScale = Vector3.one;

            stat.GetBase()["attackDamage"] = weapon.attackDamage;
            stat.Calc("attackDamage");
        }
    }
}