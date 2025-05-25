using System.Collections;
using minyee2913.Utils;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartManager : MonoBehaviour
{
    [SerializeField]
    GameObject defaultCam, readyCam, startCam;

    [SerializeField]
    GameObject defaultObj, readyText, door;

    string state = "default";
    void Start()
    {
        SoundManager.Instance.PlaySound("Dark Tension Music (No Copyright) D&D   RPG   Fantasy Music", 4, 0.2f, 1, true);
    }

    // Update is called once per frame
    void Update()
    {
        if (state == "ready")
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                state = "ready_wait";
                StartCoroutine(StartGame());

                SoundManager.Instance.PlaySound("UI/doorStart", 1, 0.6f, 1, false);
            }

            if (Input.GetKeyDown(KeyCode.Escape))
            {
                ChangeState("default", 2f);

                readyCam.SetActive(false);
                SoundManager.Instance.PlaySound("UI/doorOut", 1, 0.6f, 1, false);
            }
        }

        if (state == "ready" || state == "ready_wait")
        {
            door.transform.rotation = Quaternion.Lerp(door.transform.rotation, Quaternion.Euler(0, -110, 0), Time.deltaTime * 6);
        }
        else
        {
            door.transform.rotation = Quaternion.Lerp(door.transform.rotation, Quaternion.Euler(0, 0, 0), Time.deltaTime * 3);
        }

        defaultObj.SetActive(state == "default");
        readyText.SetActive(state == "ready");
    }

    public void Ready()
    {
        ChangeState("ready", 1.5f);

        readyCam.SetActive(true);
        SoundManager.Instance.PlaySound("UI/doorIn", 1, 0.3f, 0.8f, false);
    }

    void ChangeState(string _state, float time)
    {
        StartCoroutine(changeState(_state, time));
    }

    IEnumerator changeState(string _state, float time)
    {
        state = "wait";
        yield return new WaitForSeconds(time);

        state = _state;
    }

    IEnumerator StartGame()
    {
        startCam.SetActive(true);

        yield return new WaitForSeconds(0.2f);
        state = "start";

        yield return new WaitForSeconds(2);

        SceneManager.LoadScene("GameScene");
    }
}
