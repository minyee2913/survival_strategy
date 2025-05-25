
using minyee2913.Utils;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : Singleton<UIManager>
{
    [SerializeField]
    GameObject item;
    [SerializeField]
    Text itemName;
    [SerializeField]
    PlayerController player;

    void Update()
    {
        item.SetActive(player.equippment.weapon != null);

        if (player.equippment.weapon != null)
        {
            itemName.text = player.equippment.weapon.Name;
        }
    }
}