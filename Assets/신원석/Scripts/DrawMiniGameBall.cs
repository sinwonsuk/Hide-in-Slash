using UnityEngine;
using UnityEngine.EventSystems;

public class DrawMiniGameBall : MonoBehaviour, IDragHandler, IEndDragHandler
{

    private void Start()
    {
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

        Vector2 worldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        transform.position = worldPos;

        rigidbody2D.gravityScale = 0;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        rigidbody2D.gravityScale = 3;
    }

    private new Rigidbody2D rigidbody2D;
}
