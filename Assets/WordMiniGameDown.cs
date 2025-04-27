using UnityEngine;

public class WordMiniGameDown : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    // Update is called once per frame
    void Update()
    {
        
        rectTransform.Translate(Vector3.down * Time.deltaTime * speed);
    }

    RectTransform rectTransform;

    [SerializeField]
    float speed = 3f;
}
