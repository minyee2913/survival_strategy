
using minyee2913.Utils;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : Singleton<UIManager>
{
    [SerializeField]
    GameObject item;
    [SerializeField]
    Text itemName, atkInfo, defInfo, quickInfo;
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

        atkInfo.text = "<color=red>공격력</color> " + player.battle.stat.GetResultValue("attackDamage").ToString() + "pt";
        defInfo.text = "<color=cyan>방어력</color> " + Mathf.Round(player.battle.stat.GetResultValue("defense")).ToString() + "pt";
        quickInfo.text = "<color=orange>순발력</color> " + player.battle.stat.GetResultValue("quickness").ToString() + "%";
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