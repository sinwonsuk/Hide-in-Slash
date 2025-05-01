using UnityEngine;
using UnityEngine.EventSystems;

public class MemoryMinigameLineMove : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
   
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        LineMiniGame = GetComponentInParent<LineMiniGame>();

        boxCollider2D = GetComponent<BoxCollider2D>();
    }

   
    public void OnBeginDrag(PointerEventData eventData)
    {
       
    }

    public void OnDrag(PointerEventData eventData)
    {
        Vector2 worldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        transform.position = worldPos;
        boxCollider2D.enabled = false;
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject != gameObject && tag == collision.tag)
        {
            LineMiniGame.action.Invoke(check);
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {

        boxCollider2D.enabled = true;
        //Collider2D hit = Physics2D.OverlapPoint(transform.position);


        //if (hit != null && hit.gameObject != gameObject && tag == hit.tag)
        //{
        //    LineMiniGame.action.Invoke(check);
        //}
        //else
        //{
        //    Debug.Log($"불일치 또는 자기 자신: this: {tag}, hit: {hit?.tag}");
        //}

    }
    BoxCollider2D boxCollider2D;
    LineMiniGame LineMiniGame;

    [SerializeField]
    int check = 0;

}