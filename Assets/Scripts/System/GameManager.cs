using System;
using System.Collections;
using System.Collections.Generic;
using minyee2913.Utils;
using TMPro;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.UI;
using VRM;

public class GameManager : Singleton<GameManager>
{
    [SerializeField]
    Text timerInfo;

    public float timer, maxTime;
    public string info = "경과 시간...";
    public string state;
    public int day;

    [SerializeField]
    CinemachineCamera sleepCam, wakeCam, lookCam, showCam1, showCam2;
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
    Tomb tombPrefab;
    Vector3 deathPos;
    [SerializeField]
    List<GameObject> frames = new();


    void Start()
    {
        StartGame();
    }

    public void StartGame()
    {
        day = 1;

        StartCoroutine(wakeUp(afterFirst, new string[]{
            "음므...",
            "잠들었던건가?",
            "ㅁ... 뭐야?!!!!",
        }));
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

        timerText.transform.LookAt(PlayerController.Local.transform);
        timerText.transform.Rotate(0, 180, 0);

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

            SoundManager.Instance.PlaySound("Epic Undead Battle Music (No Copyright) D&D   RPG   Fantasy Music", 4, 0.1f, 1, true);
        }

        if (state == "fight")
        {
            if (PlayerController.Local.battle.health.isDeath)
            {
                UIManager.Instance.ShowDeath();
                state = "death";

                StartCoroutine(afterDeath());
            }
        }
    }

    IEnumerator afterDeath()
    {
        state = "";

        deathPos = PlayerController.Local.transform.position;

        yield return new WaitForSeconds(6);

        SoundManager.Instance.StopTrack(4);

        day++;

        cover.gameObject.SetActive(true);

        ifrit.health.ResetToMax();
        ifrit.gameObject.SetActive(false);

        yield return new WaitForSeconds(1);

        UIManager.Instance.HideDeath();

        StartCoroutine(wakeUp2(AfterAfter, new string[]{
            "음므...",
            "어라?",
            "나는 분명히...",
        }));

        Instantiate(tombPrefab, deathPos, Quaternion.identity);
    }

    void AfterAfter()
    {
        SoundManager.Instance.PlaySound("Dungeon Sneak Rogue Thief Music  (No Copyright) D&D   RPG   Fantasy Music", 4, 0.1f, 1, true);
        state = "ready";

        timer = 0;
        maxTime = 60;

        StartCoroutine(afterAfter());
    }

    IEnumerator afterAfter()
    {
        if (day == 2)
        {
            foreach (GameObject frame in frames)
            {
                frame.SetActive(true);
                yield return new WaitForSeconds(1.5f);
            }
            frames.ForEach((v) =>
            {
                v.SetActive(false);
            });

            yield return new WaitForSeconds(1.5f);

            message.text = "무섭지만...";

            yield return new WaitForSeconds(1.5f);


            message.text = "일단 이 해골들의 무기를 빌리자...";

            yield return new WaitForSeconds(1.5f);

            message.text = "";

            timer = 0;
            maxTime = 60;
        }
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

        yield return new WaitForSeconds(1);

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

        PlayerController.Local.animator.CutMotion(0);

        wakeCam.gameObject.SetActive(false);
        lookCam.gameObject.SetActive(true);

        message.text = msgs[2];

        PlayerController.Local.animator.blendShape.ImmediatelySetValue(BlendShapeKey.CreateFromPreset(BlendShapePreset.Sorrow), 0f);

        PlayerController.Local.movement.controller.enabled = true;

        yield return new WaitForSeconds(1f);

        message.text = "";

        lookCam.gameObject.SetActive(false);

        hud.SetActive(true);

        PlayerController.Local.animator.blendShape.ImmediatelySetValue(BlendShapeKey.CreateFromPreset(BlendShapePreset.Sorrow), 0.5f);
        PlayerController.Local.animator.blendShape.ImmediatelySetValue(BlendShapeKey.CreateUnknown("Surprised"), 0);

        PlayerController.Local.NotInControl = false;

        PlayerController.Local.animator.CutMotion(0);

        yield return new WaitForSeconds(2f);

        onEnd();
    }
}
