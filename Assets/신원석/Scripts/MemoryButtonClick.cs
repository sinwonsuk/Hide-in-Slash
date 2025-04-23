using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MemoryButtonClick : MonoBehaviour, IPointerClickHandler
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        memoryMiniGame = GetComponentInParent<MemoryMiniGame>();
    }

    // Update is called once per frame
    void Update()
    {
        //if(memoryMiniGame.activeButtonAction.Invoke() ==true)
        //{
        //    if (Input.GetMouseButtonDown(0))
        //    {
        //        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        //        RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero);
        //        if (hit.collider != null)
        //        {
        //            memoryMiniGame.checkColorAction.Invoke((int)color);
        //            Debug.Log(hit.collider.name);
        //        }
        //    }
        //}
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (memoryMiniGame.activeButtonAction.Invoke())
        {
            memoryMiniGame.checkColorAction.Invoke((int)color);
            Debug.Log($"{gameObject.name} clicked via IPointerClickHandler!");
        }
    }

    MemoryMiniGame memoryMiniGame;

    [SerializeField]
    ColorState color;
}
