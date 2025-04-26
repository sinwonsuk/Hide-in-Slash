using TMPro;
using UnityEngine;

public class StoreWindowMove : MonoBehaviour
{
    private void OnEnable()
    {
        
    }
    private void OnDisable()
    {
        uiElement.transform.position = firstPosition;
    }

    void Start()
    {
        uiElement = GetComponent<RectTransform>();
        firstPosition = uiElement.transform.position;
        targetPosition = new Vector2(0, 60);
    }

    // Update is called once per frame
    void Update()
    {
        if(uiElement.anchoredPosition.y < 60)
        {
            return;
        }

        uiElement.anchoredPosition = Vector2.Lerp(uiElement.anchoredPosition, targetPosition, Time.deltaTime * speed);
    }

    RectTransform uiElement;
    Vector2 targetPosition;

    Vector2 firstPosition;

    [SerializeField]
    float speed = 5f;
}
