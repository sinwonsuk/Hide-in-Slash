using System.Collections;
using UnityEngine;

public class robbyMove : MonoBehaviour
{

    public RectTransform uiPanel;  // 내려올 UI 패널
    public float targetY = 0f;     // 내려올 최종 Y 위치
    public float duration = 1f;    // 내려오는데 걸리는 시간


    void Start()
    {
        //코루틴시작
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

            // Ease Out Cubic: 처음엔 빠르게, 마지막에 느려짐
            float easedT = 1 - Mathf.Pow(1 - t, 3);

            uiPanel.anchoredPosition = Vector2.Lerp(startPos, endPos, easedT);
            yield return null;
        }

        uiPanel.anchoredPosition = endPos;
    }
}
