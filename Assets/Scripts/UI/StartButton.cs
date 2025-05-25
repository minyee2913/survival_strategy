using UnityEngine;
using UnityEngine.UI;

public class StartButton : MonoBehaviour
{
    [SerializeField]
    StartManager manager;
    Text text;

    Vector3 targetScale = Vector3.one;
    Color targetColor = Color.white;

    void OnEnable()
    {
        targetColor = Color.white;
    }

    void Awake()
    {
        text = GetComponent<Text>();
    }

    void Update()
    {
        transform.localScale = Vector3.Lerp(transform.localScale, targetScale, Time.deltaTime * 6);
        text.color = Color.Lerp(text.color, targetColor, Time.deltaTime * 6);
    }

    public void OnHover()
    {
        targetColor = Color.red;
        targetScale = Vector3.one * 1.1f;
    }

    public void OutHover()
    {
        targetColor = Color.white;
        targetScale = Vector3.one;
    }

    public void OnClick()
    {
        manager.Ready();
    }
}
