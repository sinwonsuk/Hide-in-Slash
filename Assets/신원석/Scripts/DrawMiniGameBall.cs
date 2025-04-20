using UnityEngine;
using UnityEngine.EventSystems;

public class DrawMiniGameBall : MonoBehaviour, IDragHandler, IEndDragHandler
{

    private void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();
        rigidbody2D = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("DrawMiniGameWall"))
        {
            Debug.Log("ÀûÀÌ¶û Ãæµ¹!");
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        Vector2 localPos;

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            rectTransform.parent.GetComponent<RectTransform>(),
            Input.mousePosition,
            canvas.worldCamera,
            out localPos
        );

        rectTransform.anchoredPosition = localPos;

        rigidbody2D.gravityScale = 0;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        rigidbody2D.gravityScale = 1;
    }

    Canvas canvas;
    RectTransform rectTransform;
    private new Rigidbody2D rigidbody2D;
}
