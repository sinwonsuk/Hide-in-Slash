using TMPro;
using UnityEngine;

public class ChattingOutput : MonoBehaviour
{
    void Awake()
    {
        textMeshProUGUI = GetComponent<TextMeshProUGUI>();
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created


    // Update is called once per frame
    void Update()
    {
        
    }

    public TextMeshProUGUI textMeshProUGUI { get; set; }

}
