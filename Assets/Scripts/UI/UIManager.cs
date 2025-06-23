
using System.Collections;
using minyee2913.Utils;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : Singleton<UIManager>
{
    protected override bool UseDontDestroyOnLoad => false;

    [SerializeField]
    GameObject item;
    [SerializeField]
    Text itemName, atkInfo, defInfo, quickInfo;
    [SerializeField]
    PlayerController player;
    [SerializeField]
    GameObject HUD, deathCam, interaction;
    [SerializeField]
    HealthIndicator indicator;
    [SerializeField]
    Text title;
    public TipPanel tip;
    public bool tipedDefense, tipedQuickness, tipedAttack;

    public void ShowTitle(string txt)
    {
        StartCoroutine(showtitle(txt));
    }

    IEnumerator showtitle(string txt)
    {
        string str = "";
        for (int i = 0; i < txt.Length; i++)
        {
            str += txt[i];

            title.text = str;

            SoundManager.Instance.PlaySound("Effect/typing", 1, 1);

            yield return new WaitForSeconds(0.2f);
        }

        yield return new WaitForSeconds(1.5f);

        title.text = "_";

        SoundManager.Instance.PlaySound("Effect/typing", 1, 1);

        yield return new WaitForSeconds(0.5f);

        title.text = "";
    }

    void Update()
    {
        item.SetActive(player.equippment.weapon != null);

        if (player.equippment.weapon != null)
        {
            itemName.text = player.equippment.weapon.Name;
        }

        indicator.gameObject.SetActive(indicator.health.gameObject.activeSelf);

        atkInfo.text = "<color=red>공격력</color> " + player.battle.stat.GetResultValue("attackDamage").ToString() + "pt";
        defInfo.text = "<color=cyan>방어력</color> " + (Mathf.Floor(player.battle.stat.GetResultValue("defense") * 10) * 0.1f).ToString() + "pt";
        quickInfo.text = "<color=orange>순발력</color> " + player.battle.stat.GetResultValue("quickness").ToString() + "%";
    }

    public void ShowDeath()
    {
        interaction.SetActive(false);
        HUD.SetActive(false);
        deathCam.SetActive(true);
    }

    public void HideDeath()
    {
        interaction.SetActive(true);
        HUD.SetActive(true);
        deathCam.SetActive(false);
    }
}