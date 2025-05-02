using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class playerDeath : MonoBehaviour
{
    [SerializeField] private float fadeDuration = 2f; // 서서히 투명해지는 시간(초)
    [SerializeField] private Image imageToFade;   // 투명하게 만들 이미지

    public void TriggerFade()
    {
        StopAllCoroutines(); 
        ResetAlpha();        
        StartCoroutine(FadeOut());
    }

    private void ResetAlpha()
    {
        if (imageToFade != null)
        {
            Color c = imageToFade.color;
            imageToFade.color = new Color(c.r, c.g, c.b, 1f);
        }
    }

    public IEnumerator FadeOut()
    {
        Color originalColor = imageToFade.color;
        float elapsedTime = 0f;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, elapsedTime / fadeDuration);
            imageToFade.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);
            yield return null;
        }

        // 완전히 투명하게 만든 뒤 alpha를 0으로 고정
        imageToFade.color = new Color(originalColor.r, originalColor.g, originalColor.b, 0f);
        imageToFade.gameObject.SetActive(false);
    }
}
