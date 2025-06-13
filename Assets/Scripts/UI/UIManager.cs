
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
    [SerializeField]
    GameObject HUD, deathCam;
    [SerializeField]
    HealthIndicator indicator;

    void Update()
    {
        item.SetActive(player.equippment.weapon != null);

        if (player.equippment.weapon != null)
        {
            itemName.text = player.equippment.weapon.Name;
        }

        indicator.gameObject.SetActive(indicator.health.gameObject.activeSelf);
    }

    public void ShowDeath()
    {
        HUD.SetActive(false);
        deathCam.SetActive(true);
    }

    public void HideDeath()
    {
        HUD.SetActive(true);
        deathCam.SetActive(false);
    }
}