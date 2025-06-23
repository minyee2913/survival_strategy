using minyee2913.Utils;
using UnityEngine;
using DG.Tweening;
using System.Collections;

public class TipPanel : UIBasePanel
{
    protected override bool closeByEscape => true;
    protected override bool openByScale => false;

    [SerializeField]
    Transform panel, outPoint;
    public GameObject defense, attack, weapon, quickness;

    public override void Open()
    {
        SoundManager.Instance.PlaySound("Effect/GUI_Notification", 1, 0.5f, 1);

        PlayerController.Local.NotInControl = true;
        defense.SetActive(false);
        quickness.SetActive(false);
        attack.SetActive(false);
        // weapon.SetActive(false);
        
        Time.timeScale = 0;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        base.Open();

        panel.transform.localPosition = outPoint.transform.localPosition;
        panel.transform.DOLocalMove(Vector3.zero, 0.5f).SetEase(Ease.InOutExpo).SetUpdate(true);
    }

    protected override void PanelUpdate()
    {
        if (Input.GetMouseButtonDown(0) && IsOpened)
        {
            Close();
        }
    }

    public override void Close()
    {
        StartCoroutine(onClose());
    }

    IEnumerator onClose()
    {
        panel.transform.DOLocalMove(outPoint.transform.localPosition, 0.5f).SetEase(Ease.InOutExpo).SetUpdate(true);

        yield return new WaitForSecondsRealtime(0.5f);

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        Time.timeScale = 1;
        base.Close();
        PlayerController.Local.NotInControl = false;
    }
}
