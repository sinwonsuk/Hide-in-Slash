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

            // ��¦�� ȿ��
            spriteRenderer.enabled = true;
            yield return new WaitForSeconds(0.1f);
            spriteRenderer.enabled = false;
            yield return new WaitForSeconds(0.1f);
            spriteRenderer.enabled = true;

            // ���İ� ���� ���̱�
            float elapsed = 0f;
            Color color = spriteRenderer.color;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float alpha = Mathf.Lerp(1f, 0f, elapsed / duration);
                spriteRenderer.color = new Color(color.r, color.g, color.b, alpha);
                yield return null;
            }

            // ���� �ʱ�ȭ
            spriteRenderer.color = new Color(color.r, color.g, color.b, 1f);
            spriteRenderer.enabled = false;  // ��������Ʈ �����
        }
    }
}
