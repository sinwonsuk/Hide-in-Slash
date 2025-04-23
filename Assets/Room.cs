using UnityEngine;

public class Room : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rectTransform =GetComponent<RectTransform>();   
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    RectTransform rectTransform;

}
