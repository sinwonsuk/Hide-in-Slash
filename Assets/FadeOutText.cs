using TMPro;
using UnityEngine;

public class FadeOutText : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        generator = GetComponent<TextMeshProUGUI>();
    }

    // Update is called once per frame
    void Update()
    {
        if (generator.color.a <= 0)
        {
            Destroy(gameObject);
        }

        Color color = generator.color;

        float newAlpha = Mathf.Lerp(color.a, targetAlpha, Time.deltaTime * fadeSpeed);
        color.a = newAlpha;

        generator.color = color;
    }

    TextMeshProUGUI generator;
    float targetAlpha = 0f;
    float fadeSpeed = 1f;
}
