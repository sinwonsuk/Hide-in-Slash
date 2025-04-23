using System.Collections;
using UnityEngine;

public class robbyMove : MonoBehaviour
{

    public RectTransform uiPanel;  // ������ UI �г�
    public float targetY = 0f;     // ������ ���� Y ��ġ
    public float duration = 1f;    // �������µ� �ɸ��� �ð�


    void Start()
    {
        //�ڷ�ƾ����
        StartCoroutine(MoveUI());
    }

    IEnumerator MoveUI()
    {
        Vector2 startPos = uiPanel.anchoredPosition;
        Vector2 endPos = new Vector2(startPos.x, targetY);
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);

            // Ease Out Cubic: ó���� ������, �������� ������
            float easedT = 1 - Mathf.Pow(1 - t, 3);

            uiPanel.anchoredPosition = Vector2.Lerp(startPos, endPos, easedT);
            yield return null;
        }

        uiPanel.anchoredPosition = endPos;
    }
}
