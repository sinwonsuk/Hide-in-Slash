using System;
using UnityEngine;
using UnityEngine.UI;

public class ChattingManager : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        wordhandler = CreateChat;
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Return) && isActiveInputWindow==false)
        {
            isActiveInputWindow = true;
            chattingInputWindow.SetActive(true);
        }
        else if(Input.GetKeyDown(KeyCode.Return) && isActiveInputWindow == true)
        {
            isActiveInputWindow = false;
            chattingInputWindow.SetActive(false);
        }
    }

    public void CreateChat(string _text)
    {
        //서버추가하면 이때 받으면 될듯

        GameObject instantiate = Instantiate(chattingObject, transform);

        ChattingOutput chattingOutput = instantiate.GetComponent<ChattingOutput>();

        chattingOutput.textMeshProUGUI.text = _text;

        Canvas.ForceUpdateCanvases();
        scrollRect.verticalNormalizedPosition = 0f; 
    }

    [SerializeField]
    private ScrollRect scrollRect;

    public Action<string> wordhandler;

    [SerializeField]
    private GameObject chattingObject;

    [SerializeField]
    private GameObject chattingInputWindow;

    bool isActiveInputWindow;

}
