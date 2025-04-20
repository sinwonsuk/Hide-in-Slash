using UnityEngine;
using UnityEngine.UI;

public class MemoryButtonClick : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        memoryMiniGame = GetComponentInParent<MemoryMiniGame>();
        button = GetComponent<Button>();
    }

    // Update is called once per frame
    void Update()
    {
        if(memoryMiniGame.ActiveButton())
        {
            button.enabled = true;
        }
    }
    Button button;
    MemoryMiniGame memoryMiniGame;
}
