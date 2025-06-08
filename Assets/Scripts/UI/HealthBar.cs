using System.Collections;
using minyee2913.Utils;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [SerializeField]
    Slider slider;
    [SerializeField]
    HealthObject hp;
    [SerializeField]
    Image fillRect;
    float rateBefore;

    void Update()
    {
        slider.value = hp.Rate;

        if (hp.Rate != rateBefore)
        {
            if (rateBefore > hp.Rate)
            {
                StartCoroutine(WhenChanged());
            }

            rateBefore = hp.Rate;
        }
    }
    
    IEnumerator WhenChanged()
    {
        float timeIn = 0.01f;
        float timeWait = 0f;
        float timeOut = 0.01f;

        Color originalColor = Color.white;  // 원래 색상
        Color targetColor = Color.red;  // 변할 색상

        // 커질 때 & 색상 빨강 -> 흰색
        for (int i = 0; i <= 20; i++)
        {
            float t = i / 20f;
            fillRect.transform.localScale = Vector3.one * (1 + 0.05f * t);
            fillRect.color = Color.Lerp(originalColor, targetColor, t);

            yield return new WaitForSeconds(timeIn / 20f);
        }

        yield return new WaitForSeconds(timeWait);

        // 작아질 때 & 색상 흰색 -> 빨강
        for (int i = 0; i <= 20; i++)
        {
            float t = i / 20f;
            fillRect.transform.localScale = Vector3.one * (1 + 0.05f * (1 - t));
            fillRect.color = Color.Lerp(targetColor, originalColor, t);

            yield return new WaitForSeconds(timeOut / 20f);
        }
    }
}