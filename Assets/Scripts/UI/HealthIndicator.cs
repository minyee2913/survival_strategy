using System.Collections;
using minyee2913.Utils;
using UnityEngine;
using UnityEngine.UI;

public class HealthIndicator : MonoBehaviour
{
    [SerializeField]
    Image displayRate, whiteMark;
    [SerializeField]
    Text rateDisplay;

    [SerializeField]
    HealthObject health;
    Color color;

    float rateBefore;

    void Start()
    {
        rateBefore = health.Rate;
        color = displayRate.color;
    }

    void Update()
    {
        MatchDisplay();

        if (health.Rate != rateBefore)
        {
            if (rateBefore > health.Rate)
            {
                StartCoroutine(WhenChanged());
            }

            rateBefore = health.Rate;
        }
    }

    void MatchDisplay()
    {
        rateDisplay.text = Mathf.Floor(health.Rate * 100).ToString() + "%";
        displayRate.fillAmount = health.Rate;
        whiteMark.transform.localRotation = Quaternion.Euler(0, 0, -360 * health.Rate);
    }

    IEnumerator WhenChanged()
    {
        float timeIn = 0.01f;
        float timeWait = 0f;
        float timeOut = 0.01f;

        Color originalColor = color;  // 원래 색상
        Color targetColor = Color.white;  // 변할 색상

        // 커질 때 & 색상 빨강 -> 흰색
        for (int i = 0; i <= 20; i++)
        {
            float t = i / 20f;
            displayRate.transform.localScale = Vector3.one * (1 + 0.05f * t);
            displayRate.color = Color.Lerp(originalColor, targetColor, t);

            yield return new WaitForSeconds(timeIn / 20f);
        }

        yield return new WaitForSeconds(timeWait);

        // 작아질 때 & 색상 흰색 -> 빨강
        for (int i = 0; i <= 20; i++)
        {
            float t = i / 20f;
            displayRate.transform.localScale = Vector3.one * (1 + 0.05f * (1 - t));
            displayRate.color = Color.Lerp(targetColor, originalColor, t);

            yield return new WaitForSeconds(timeOut / 20f);
        }
    }

}
