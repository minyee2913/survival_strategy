using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class VictoryManager : MonoBehaviour
{
    public static float playTime;
    public static int death;
    public List<GameObject> cams;
    public List<Text> texts;
    public Text author;

    bool canNext;

    void Start()
    {
        StartCoroutine(effects());

        int minutes = Mathf.FloorToInt(playTime / 60f);
        int seconds = Mathf.FloorToInt(playTime % 60f);

        string timeFormatted = string.Format("{0:00}:{1:00}", minutes, seconds);
        texts[1].text = "죽은 횟수: 2493 + " + death.ToString();
        texts[2].text = "플레이 타임 " + timeFormatted;

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    void Update()
    {
        if (canNext)
        {
            if (Input.anyKeyDown)
            {
                SceneManager.LoadScene("MainScene");
            }
        }
    }

    void ShowText(int index)
    {
        foreach (Text txt in texts)
        {
            txt.gameObject.SetActive(false);
        }

        texts[index].gameObject.SetActive(true);
    }

    IEnumerator effects()
    {
        yield return new WaitForSeconds(7);

        cams[0].SetActive(true);
        ShowText(0);

        yield return new WaitForSeconds(2);

        cams[1].SetActive(true);
        ShowText(1);

        yield return new WaitForSeconds(2);

        cams[2].SetActive(true);
        ShowText(2);

        yield return new WaitForSeconds(3);

        cams[3].SetActive(true);
        ShowText(3);
        author.text = "";

        yield return new WaitForSeconds(1f);
        author.text = "minyee2913";

        yield return new WaitForSeconds(0.5f);
        author.text = "30511";
        author.color = Color.white;

        yield return new WaitForSeconds(0.5f);
        author.text = "30511 이강민";
        author.color = Color.cyan;

        yield return new WaitForSeconds(1f);

        texts.ForEach((t) =>
        {
            t.gameObject.SetActive(true);
        });

        canNext = true;
    }
}
