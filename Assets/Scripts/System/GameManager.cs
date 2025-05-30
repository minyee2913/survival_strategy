using minyee2913.Utils;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    Text timerInfo, timerText;

    public float timer, maxTime;
    public string info = "경과 시간...";
    public string state;

    void Start()
    {
        StartGame();
    }

    public void StartGame()
    {
        SoundManager.Instance.PlaySound("Dungeon Sneak Rogue Thief Music  (No Copyright) D&D   RPG   Fantasy Music", 4, 0.1f, 1, true);
        state = "ready";

        timer = 0;

        info = "보스가 돌아올때까지...";
    }

    void Update()
    {
        timer += Time.deltaTime;

        float time = timer;
        if (maxTime > 0)
        {
            time = maxTime - timer;
        }

        int minutes = Mathf.FloorToInt(time / 60f);
        int seconds = Mathf.FloorToInt(time % 60f);

        string timeFormatted = string.Format("{0:00}:{1:00}", minutes, seconds);

        timerText.text = timeFormatted;
        timerInfo.text = info;
    }
}
