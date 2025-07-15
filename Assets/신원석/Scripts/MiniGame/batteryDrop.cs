using UnityEngine;

public class batteryDrop : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    // Update is called once per frame
    void Update()
    {
        if(wordRectTransform == null)
        {
            Destroy(gameObject);
            return;
        }

        rectTransform.anchoredPosition = wordRectTransform.anchoredPosition;
    }

    RectTransform rectTransform;

    [SerializeField]
    RectTransform wordRectTransform;


    float speed = 5.0f;   
}
