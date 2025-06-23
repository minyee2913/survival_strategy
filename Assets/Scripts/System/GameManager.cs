using System;
using System.Collections;
using System.Collections.Generic;
using minyee2913.Utils;
using TMPro;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using VRM;

public class GameManager : Singleton<GameManager>
{
    protected override bool UseDontDestroyOnLoad => false;

    [SerializeField]
    Text timerInfo;

    public float timer, maxTime, playTime;
    public string info = "경과 시간...";
    public string state;
    public int day, death;

    [SerializeField]
    CinemachineCamera sleepCam, wakeCam, lookCam, showCam1, showCam2, daddddddCam;
    [SerializeField]
    Image cover;
    public Text message;
    [SerializeField]
    TMP_Text timerText;
    [SerializeField]
    GameObject hud, explosion;
    [SerializeField]
    Ifrit ifrit;
    [SerializeField]
    Tomb tombPrefab, tombSpecial;
    Vector3 deathPos;
    [SerializeField]
    List<GameObject> frames = new(), frames2 = new();
    [SerializeField]
    Volume glitch;

    bool facedTomb;


    void Start()
    {
        StartGame();
    }

    public void StartGame()
    {
        playTime = 0;
        facedTomb = false;
        ifrit.gameObject.SetActive(false);
        day = 1;
        death = 0;

        PlayerController.Local.battle.stat.ResetStat();
        PlayerController.Local.battle.health.ResetToMax();

        StartCoroutine(wakeUp(afterFirst, new string[]{
            "음므...",
            "잠들었던건가?",
            "ㅁ... 뭐야?!!!!",
        }));
    }

    void Update()
    {
        timer += Time.deltaTime;
        playTime += Time.deltaTime;

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

        timerText.transform.LookAt(PlayerController.Local.transform);
        timerText.transform.Rotate(0, 180, 0);

        if (day == 3 && !facedTomb && Vector3.Distance(PlayerController.Local.transform.position, deathPos) <= 3)
        {
            StartCoroutine(afterFaced());
        }

        if (state == "ready" && timer >= maxTime)
        {
            timerText.gameObject.SetActive(false);

            ifrit.gameObject.SetActive(true);
            ifrit.health.isDeath = false;
            ifrit.health.Health = ifrit.health.MaxHealth;

            ifrit.transform.position = timerText.transform.position;

            ifrit.state = "idle";

            StartCoroutine(ShowExplosion());

            state = "fight";

            if (day >= 2)
            {
                if (!UIManager.Instance.tipedAttack)
                {
                    UIManager.Instance.tip.Open();
                    UIManager.Instance.tip.attack.SetActive(true);

                    UIManager.Instance.tipedAttack = true;
                }
            }

            CamEffector.current.Shake(8, 0.3f);


            if (day < 3)
            {
                UIManager.Instance.ShowTitle("???");
                SoundManager.Instance.PlaySound("Epic Undead Battle Music (No Copyright) D&D   RPG   Fantasy Music", 4, 0.1f, 1, true);
            }
            else
            {
                UIManager.Instance.ShowTitle("이프리트 - 불꽃을 휘감은 자");
                SoundManager.Instance.PlaySound("Swimming With Sharks (Instrumental)", 4, 0.1f, 1, true);
            }
        }

        if (state == "fight")
        {
            if (ifrit.health.isDeath) {
                if (day < 3)
                {
                    PlayerController.Local.battle.health.Kill();
                }
                else
                {
                    state = "win";

                    StartCoroutine(afterWin());
                }
            }

            if (PlayerController.Local.battle.health.isDeath)
            {
                UIManager.Instance.ShowDeath();
                state = "death";

                StartCoroutine(afterDeath());
            }
        }
    }

    IEnumerator afterWin()
    {
        SoundManager.Instance.StopTrack(4);
        ifrit.gameObject.SetActive(false);
        yield return new WaitForSeconds(1.5f);

        message.text = "어..?";

        yield return new WaitForSeconds(2f);

        message.text = "끝난거야?";

        yield return new WaitForSeconds(2f);

        message.text = "진짜로 내가 이긴거야?!";

        yield return new WaitForSeconds(3f);

        PlayerController.Local.animator.Trigger("Win");

        message.text = "야호!";

        yield return new WaitForSeconds(2f);

        message.text = "헤헤 별것도 아니였네";

        yield return new WaitForSeconds(1.5f);

        message.text = "이제 돌아갈 수 있겠...";

        SoundManager.Instance.PlaySound("Effect/glitch", 1, 0.8f, 1);

        yield return new WaitForSeconds(0.1f);

        for (int i = 0; i <= 10; i++)
        {
            glitch.weight = i * 0.1f;

            yield return new WaitForSeconds(0.05f);
        }

        glitch.weight = 1;

        cover.gameObject.SetActive(true);
        cover.color = Color.black;

        message.text = "";

        VictoryManager.death = death;
        VictoryManager.playTime = playTime;

        yield return new WaitForSeconds(3f);

        SceneManager.LoadScene("Victory");
    }

    IEnumerator afterFaced()
    {
        facedTomb = true;

        timer = 0;
        maxTime = 40;

        PlayerController.Local.transform.LookAt(deathPos);

        PlayerController.Local.NotInControl = true;
        PlayerController.Local.animator.ResetWait();
        message.text = "이게 뭐야...";

        yield return new WaitForSeconds(1.5f);

        message.text = "이 해골 아까는 없었는데";
        PlayerController.Local.animator.ResetWait();

        yield return new WaitForSeconds(2f);

        message.text = "설마...";

        yield return new WaitForSeconds(2f);

        SoundManager.Instance.PlaySound("Effect/surprised", 2, 0.3f, 1f, false);

        UIManager.Instance.ShowDeath();

        daddddddCam.gameObject.SetActive(true);

        CamEffector.current.ViewUp(-5, 0, 0.3f);

        message.text = "이거 나야?";

        PlayerController.Local.animator.blendShape.ImmediatelySetValue(BlendShapeKey.CreateUnknown("Surprised"), 1);

        PlayerController.Local.animator.ResetWait();

        yield return new WaitForSeconds(1.8f);

        CamEffector.current.ViewUp(-10, 0, 0.3f);

        message.text = "여기 있는게 전부?";

        yield return new WaitForSeconds(2f);

        SoundManager.Instance.PauseTrack(4);

        cover.gameObject.SetActive(true);
        cover.color = Color.black;

        UIManager.Instance.HideDeath();

        CamEffector.current.ViewOut(0f);

        daddddddCam.gameObject.SetActive(false);

        message.text = "...";

        yield return new WaitForSeconds(1.5f);

        message.text = "어떻게든 탈출하고 말겠어!";

        PlayerController.Local.transform.rotation = Quaternion.Euler(PlayerController.Local.transform.rotation.x, PlayerController.Local.transform.rotation.y, 0);

        yield return new WaitForSeconds(1.5f);

        PlayerController.Local.animator.blendShape.ImmediatelySetValue(BlendShapeKey.CreateUnknown("Surprised"), 0);

        SoundManager.Instance.ResumeTrack(4);

        message.text = "";

        cover.gameObject.SetActive(false);

        state = "ready";

        PlayerController.Local.NotInControl = false;

        yield return new WaitForSeconds(2f);

        UIManager.Instance.tip.Open();
        UIManager.Instance.tip.weapon.SetActive(true);
    }

    IEnumerator afterDeath()
    {
        death++;

        state = "";

        deathPos = PlayerController.Local.transform.position;

        yield return new WaitForSeconds(6);

        SoundManager.Instance.StopTrack(4);

        day++;

        SoundManager.Instance.PlaySound("Effect/glitch", 1, 0.8f, 1);

        yield return new WaitForSeconds(0.1f);

        for (int i = 0; i <= 10; i++)
        {
            glitch.weight = i * 0.1f;

            yield return new WaitForSeconds(0.05f);
        }

        glitch.weight = 1;

        cover.gameObject.SetActive(true);
        cover.color = Color.black;

        ifrit.health.ResetToMax();
        ifrit.gameObject.SetActive(false);

        yield return new WaitForSeconds(1);

        glitch.weight = 0;

        UIManager.Instance.HideDeath();

        if (day == 2)
        {
            StartCoroutine(wakeUp2(AfterAfter, new string[]{
                "음므...",
                "어라?",
                "나는 분명히...",
            }));
        }
        else if (day == 3)
        {
            StartCoroutine(wakeUp2(AfterAfter, new string[]{
                "...",
                "또...",
                "다시 돌아왔어",
            }));
        }
        else
        {
            StartCoroutine(wakeUp2(AfterAfter, new string[]{
                "",
                "",
                "",
            }));
        }

        PlayerController.Local.equippment.Equip(null);

        if (day <= 2)
        {
            Instantiate(tombPrefab, deathPos, Quaternion.identity);
        }
        else
        {
            Instantiate(tombSpecial, deathPos, Quaternion.identity);
        }
    }

    void AfterAfter()
    {
        state = "none";

        timer = 0;
        maxTime = 0;

        timerText.gameObject.SetActive(true);

        StartCoroutine(afterAfter());
    }

    IEnumerator afterAfter()
    {
        if (day == 2)
        {
            foreach (GameObject frame in frames)
            {
                frame.SetActive(true);

                if (frame.name == "frame2")
                    SoundManager.Instance.PlaySound("Effect/dirty", 1, 0.2f, 1);
                if (frame.name == "frame3")
                    SoundManager.Instance.PlaySound("Effect/police", 1, 0.3f, 1);
                if (frame.name == "frame5")
                    SoundManager.Instance.PlaySound("Effect/alarm", 1, 0.5f, 1);

                yield return new WaitForSeconds(2f);
            }
            frames.ForEach((v) =>
            {
                v.SetActive(false);
            });

            SoundManager.Instance.StopTrack(1);

            yield return new WaitForSeconds(1.5f);

            message.text = "무섭지만...";

            yield return new WaitForSeconds(1.5f);


            message.text = "일단 이 해골들의 무기를 빌리자...";

            yield return new WaitForSeconds(1.5f);

            message.text = "";

            state = "ready";

            timer = 0;
            maxTime = 60;

            SoundManager.Instance.PlaySound("Dungeon Sneak Rogue Thief Music  (No Copyright) D&D   RPG   Fantasy Music", 4, 0.1f, 1, true);
        }
        else if (day == 3)
        {
            foreach (GameObject frame in frames2)
            {
                frame.SetActive(true);

                if (frame.name == "frame2")
                    SoundManager.Instance.PlaySound("Effect/train-crossing-105569", 1, 0.5f, 1);
                if (frame.name == "frame3")
                    SoundManager.Instance.PlaySound("Effect/clock", 1, 1f, 1);
                if (frame.name == "frame5")
                    SoundManager.Instance.PlaySound("Effect/Train2", 1, 0.8f, 1);

                yield return new WaitForSeconds(2f);
            }
            frames2.ForEach((v) =>
            {
                v.SetActive(false);
            });

            SoundManager.Instance.PlaySound("From The Shadows", 4, 0.1f, 1, true);

            message.text = "일단 아까 쓰러졌던 곳에 가보자.";

            yield return new WaitForSeconds(1.5f);

            message.text = "";

            state = "finding";

            timer = 0;
            maxTime = 0;
        }
        else
        {
            state = "ready";

            timer = 0;
            maxTime = 60;

            SoundManager.Instance.PlaySound("Thieving Rogue Sneak Battle Music (No Copyright) D&D   RPG   Fantasy Music", 4, 0.1f, 1, true);
        }

        UIManager.Instance.ShowTitle(day.ToString() + "회차_");

        yield break;
    }

    IEnumerator ShowExplosion()
    {
        explosion.transform.position = timerText.transform.position;
        explosion.SetActive(true);

        yield return new WaitForSeconds(2);

        explosion.SetActive(false);
    }

    void afterFirst()
    {
        SoundManager.Instance.PlaySound("Dungeon Sneak Rogue Thief Music  (No Copyright) D&D   RPG   Fantasy Music", 4, 0.1f, 1, true);
        state = "ready";

        timer = 0;
        maxTime = 60;
    }

    IEnumerator wakeUp(Action onEnd, string[] msgs)
    {
        PlayerController.Local.NotInControl = true;
        PlayerController.Local.movement.Teleport(new Vector3(-12, 0.5f, -32.2f));
        PlayerController.Local.movement.controller.enabled = false;
        PlayerController.Local.transform.localRotation = Quaternion.Euler(0, 0, 0);
        sleepCam.gameObject.SetActive(true);

        cover.color = Color.black;
        cover.gameObject.SetActive(true);
        hud.SetActive(false);

        PlayerController.Local.animator.blendShape.ImmediatelySetValue(BlendShapeKey.CreateFromPreset(BlendShapePreset.Blink), 1);
        PlayerController.Local.animator.blendShape.ImmediatelySetValue(BlendShapeKey.CreateFromPreset(BlendShapePreset.Sorrow), 0.6f);
        PlayerController.Local.animator.CutMotion(1);

        yield return new WaitForSeconds(1);

        PlayerController.Local.animator.blendShape.ImmediatelySetValue(BlendShapeKey.CreateFromPreset(BlendShapePreset.Blink), 1);
        PlayerController.Local.animator.blendShape.ImmediatelySetValue(BlendShapeKey.CreateFromPreset(BlendShapePreset.Sorrow), 0.6f);

        Color black = Color.black;
        for (int i = 0; i < 20; i++)
        {
            black.a = (20 - i) / 20f;
            cover.color = black;

            yield return new WaitForSeconds(1 / 20f);
        }

        yield return new WaitForSeconds(3);

        float time = 0;
        while (time < 0.5f)
        {
            PlayerController.Local.animator.blendShape.ImmediatelySetValue(BlendShapeKey.CreateFromPreset(BlendShapePreset.Blink), 0.5f - time);

            time += Time.deltaTime;
            yield return null;
        }

        time = 0.5f;
        while (time > 0f)
        {
            PlayerController.Local.animator.blendShape.ImmediatelySetValue(BlendShapeKey.CreateFromPreset(BlendShapePreset.Blink), 1f - time * 2);

            time -= Time.deltaTime;
            yield return null;
        }

        yield return new WaitForSeconds(0.2f);

        time = 0;
        while (time < 0.5f)
        {
            PlayerController.Local.animator.blendShape.ImmediatelySetValue(BlendShapeKey.CreateFromPreset(BlendShapePreset.Blink), 0.5f - time);

            time += Time.deltaTime;
            yield return null;
        }

        PlayerController.Local.animator.blendShape.ImmediatelySetValue(BlendShapeKey.CreateFromPreset(BlendShapePreset.Blink), 0);

        message.text = msgs[0];

        yield return new WaitForSeconds(2f);

        PlayerController.Local.animator.CutMotion(0);

        sleepCam.gameObject.SetActive(false);
        wakeCam.gameObject.SetActive(true);

        message.text = msgs[1];

        yield return new WaitForSeconds(3f);

        PlayerController.Local.animator.CutMotion(0);

        wakeCam.gameObject.SetActive(false);
        lookCam.gameObject.SetActive(true);

        message.text = msgs[2];

        PlayerController.Local.animator.blendShape.ImmediatelySetValue(BlendShapeKey.CreateFromPreset(BlendShapePreset.Sorrow), 0f);

        SoundManager.Instance.PlaySound("Effect/surprised", 2, 0.3f, 1f, false);

        time = 0;
        while (time < 1f)
        {
            PlayerController.Local.animator.blendShape.ImmediatelySetValue(BlendShapeKey.CreateUnknown("Surprised"), time);

            time += Time.deltaTime;
            yield return null;
        }

        yield return new WaitForSeconds(1f);

        PlayerController.Local.movement.controller.enabled = true;

        message.text = "";

        SoundManager.Instance.PlaySound("Effect/scary", 1, 0.3f, 1f, false);

        lookCam.gameObject.SetActive(false);
        showCam1.gameObject.SetActive(true);

        yield return new WaitForSeconds(0.3f);

        showCam1.gameObject.SetActive(false);
        showCam2.gameObject.SetActive(true);

        yield return new WaitForSeconds(10f);

        showCam2.gameObject.SetActive(false);

        hud.SetActive(true);

        PlayerController.Local.animator.blendShape.ImmediatelySetValue(BlendShapeKey.CreateFromPreset(BlendShapePreset.Sorrow), 0.5f);
        PlayerController.Local.animator.blendShape.ImmediatelySetValue(BlendShapeKey.CreateUnknown("Surprised"), 0);

        PlayerController.Local.NotInControl = false;

        PlayerController.Local.animator.CutMotion(0);

        yield return new WaitForSeconds(2f);

        onEnd();
    }

    IEnumerator wakeUp2(Action onEnd, string[] msgs)
    {
        PlayerController.Local.NotInControl = true;
        PlayerController.Local.movement.Teleport(new Vector3(-12, 0.5f, -32.2f));
        PlayerController.Local.movement.controller.enabled = false;
        PlayerController.Local.transform.localRotation = Quaternion.Euler(0, 0, 0);
        sleepCam.gameObject.SetActive(true);

        cover.color = Color.black;
        cover.gameObject.SetActive(true);
        hud.SetActive(false);

        PlayerController.Local.animator.blendShape.ImmediatelySetValue(BlendShapeKey.CreateFromPreset(BlendShapePreset.Blink), 1);
        PlayerController.Local.animator.blendShape.ImmediatelySetValue(BlendShapeKey.CreateFromPreset(BlendShapePreset.Sorrow), 0.6f);
        PlayerController.Local.animator.CutMotion(1);

        yield return new WaitForSeconds(3);

        PlayerController.Local.battle.health.ResetToMax();

        Color black = Color.black;
        for (int i = 0; i < 20; i++)
        {
            black.a = (20 - i) / 20f;
            cover.color = black;

            yield return new WaitForSeconds(1 / 20f);
        }

        yield return new WaitForSeconds(3);

        float time = 0;
        while (time < 0.5f)
        {
            PlayerController.Local.animator.blendShape.ImmediatelySetValue(BlendShapeKey.CreateFromPreset(BlendShapePreset.Blink), 0.5f - time);

            time += Time.deltaTime;
            yield return null;
        }

        time = 0.5f;
        while (time > 0f)
        {
            PlayerController.Local.animator.blendShape.ImmediatelySetValue(BlendShapeKey.CreateFromPreset(BlendShapePreset.Blink), 1f - time * 2);

            time -= Time.deltaTime;
            yield return null;
        }

        yield return new WaitForSeconds(0.2f);

        time = 0;
        while (time < 0.5f)
        {
            PlayerController.Local.animator.blendShape.ImmediatelySetValue(BlendShapeKey.CreateFromPreset(BlendShapePreset.Blink), 0.5f - time);

            time += Time.deltaTime;
            yield return null;
        }

        PlayerController.Local.animator.blendShape.ImmediatelySetValue(BlendShapeKey.CreateFromPreset(BlendShapePreset.Blink), 0);

        message.text = msgs[0];

        yield return new WaitForSeconds(2f);

        PlayerController.Local.animator.CutMotion(0);

        sleepCam.gameObject.SetActive(false);
        wakeCam.gameObject.SetActive(true);

        message.text = msgs[1];

        yield return new WaitForSeconds(3f);

        message.text = msgs[2];

        PlayerController.Local.animator.blendShape.ImmediatelySetValue(BlendShapeKey.CreateFromPreset(BlendShapePreset.Sorrow), 0f);

        PlayerController.Local.animator.CutMotion(0);

        yield return new WaitForSeconds(1f);

        PlayerController.Local.movement.controller.enabled = true;

        onEnd();

        message.text = "";

        wakeCam.gameObject.SetActive(false);

        hud.SetActive(true);

        PlayerController.Local.animator.blendShape.ImmediatelySetValue(BlendShapeKey.CreateFromPreset(BlendShapePreset.Sorrow), 0.5f);
        PlayerController.Local.animator.blendShape.ImmediatelySetValue(BlendShapeKey.CreateUnknown("Surprised"), 0);

        PlayerController.Local.NotInControl = false;
        PlayerController.Local.animator.CutMotion(0);

        yield return new WaitForSeconds(2f);
    }
}
