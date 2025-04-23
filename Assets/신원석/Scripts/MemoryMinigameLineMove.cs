using UnityEngine;
using UnityEngine.EventSystems;

public class MemoryMinigameLineMove : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
   
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        LineMiniGame = GetComponentInParent<LineMiniGame>();
    }

   
    public void OnBeginDrag(PointerEventData eventData)
    {
       
    }

    public void OnDrag(PointerEventData eventData)
    {
        Vector2 worldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        transform.position = worldPos;

    }

    public void OnEndDrag(PointerEventData eventData)
    {
        Collider2D hit = Physics2D.OverlapPoint(transform.position);
        if (hit != null && this.CompareTag(hit.tag))
        {
            LineMiniGame.action.Invoke(check);
        }     
    }

    LineMiniGame LineMiniGame;

    [SerializeField]
    int check = 0;

}