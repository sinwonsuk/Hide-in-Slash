using UnityEngine;
using System.Collections;

public class titleBackGround : MonoBehaviour
{
    public SpriteRenderer spriteRenderer;
    public float duration = 1f;

    void Start()
    {
        spriteRenderer.enabled = false;
        StartCoroutine(backGroundAnimation());
    }

    public IEnumerator backGroundAnimation()
    {
        while (true)
        {
            yield return new WaitForSeconds(3f);

            // 반짝임 효과
            spriteRenderer.enabled = true;
            yield return new WaitForSeconds(0.1f);
            spriteRenderer.enabled = false;
            yield return new WaitForSeconds(0.1f);
            spriteRenderer.enabled = true;

            // 알파값 점점 줄이기
            float elapsed = 0f;
            Color color = spriteRenderer.color;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float alpha = Mathf.Lerp(1f, 0f, elapsed / duration);
                spriteRenderer.color = new Color(color.r, color.g, color.b, alpha);
                yield return null;
            }

            // 알파 초기화
            spriteRenderer.color = new Color(color.r, color.g, color.b, 1f);
            spriteRenderer.enabled = false;  // 스프라이트 숨기기
        }
    }
}
