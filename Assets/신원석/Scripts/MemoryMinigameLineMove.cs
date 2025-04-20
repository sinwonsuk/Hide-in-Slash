using UnityEngine;
using UnityEngine.EventSystems;

public class MemoryMinigameLineMove : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
   
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        LineMiniGame = GetComponentInParent<LineMiniGame>();
        rectTransform = GetComponent<RectTransform>();  
        canvas = GetComponentInParent<Canvas>();  
    }

   
    public void OnBeginDrag(PointerEventData eventData)
    {
       
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
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        Collider2D hit = Physics2D.OverlapPoint(rectTransform.position);
        if (hit != null && this.CompareTag(hit.tag))
        {
            LineMiniGame.action.Invoke(check);
        }     
    }

    RectTransform rectTransform;
    Canvas canvas;
    LineMiniGame LineMiniGame;

    [SerializeField]
    int check = 0;

}