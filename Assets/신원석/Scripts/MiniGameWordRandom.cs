using NUnit.Framework;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MiniGameWordDrop : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
      
        textMeshProUGUI = GetComponent<TextMeshProUGUI>();

        int randomindex = Random.Range(0, 20);

        textMeshProUGUI.text = List[randomindex];

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    [SerializeField]    
    List<string> List = new List<string>();

    [SerializeField]
    float speed = 100.0f;

    public TextMeshProUGUI textMeshProUGUI;

}
