using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TestGradient : MonoBehaviour
{
    public TextMeshProUGUI uiText;
    public Color startColor = Color.green;
    public Color endColor = Color.yellow;
    public float lerpSpeed = 1f;

    void Update()
    {
        //float t = Mathf.PingPong(Time.time * lerpSpeed, 1f);
        //float t = Mathf.Repeat(Time.time * lerpSpeed, 1f);
        float t = (Mathf.Sin(Time.time * lerpSpeed) + 1f) / 2f;
        uiText.color = Color.Lerp(startColor, endColor, t);
    }
}