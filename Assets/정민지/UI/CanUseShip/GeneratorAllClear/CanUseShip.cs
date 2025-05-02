using TMPro;
using UnityEngine;
using System.Collections;

public class CanUseShip : MonoBehaviour
{

    public GameObject canvasPrefab; // ✅ 프리팹을 인스펙터에서 할당
    private GameObject canvasInstance;
    private TMP_Text text;

    [SerializeField] private float fadeDuration = 2f;
    [SerializeField] private float visibleDuration = 1f;
    private Animator anim;

    void Start()
    {
        anim = GetComponent<Animator>();

        // 캔버스를 독립적으로 생성
        canvasInstance = Instantiate(canvasPrefab);
        text = canvasInstance.GetComponentInChildren<TMP_Text>();

        // 캔버스 위치를 부모의 위치로 설정 (부모의 위치를 추적하려면 Update에서 처리)
        UpdateCanvasPosition();
    }

    void Update()
    {
        // 부모의 위치를 따라가도록 캔버스 위치 업데이트
        UpdateCanvasPosition();
    }

    void UpdateCanvasPosition()
    {
        if (canvasInstance != null)
        {
            // 캔버스를 부모의 위치와 동일하게 맞춰줌
            canvasInstance.transform.position = transform.position;
        }
    }

    public void WaitAndStart()
    {
        StartCoroutine(FadeInOut());
    }

    IEnumerator FadeInOut()
    {
        yield return StartCoroutine(Fade(0f, 1f, fadeDuration));
        yield return new WaitForSeconds(visibleDuration);
        anim.SetBool("Close", true);
        yield return StartCoroutine(Fade(1f, 0f, fadeDuration));
    }

    IEnumerator Fade(float from, float to, float duration)
    {
        float timer = 0f;
        Color color = text.color;

        while (timer < duration)
        {
            timer += Time.deltaTime;
            float alpha = Mathf.Lerp(from, to, timer / duration);
            color.a = alpha;
            text.color = color;
            yield return null;
        }

        color.a = to;
        text.color = color;
    }

    public void DestroyInfoObject()
    {
        Destroy(gameObject);
        Destroy(canvasInstance);
    }
}



    
