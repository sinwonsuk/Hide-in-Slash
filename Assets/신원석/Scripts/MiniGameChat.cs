using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MiniGameChat : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created  
    void Start()
    {
        inputField = GetComponent<InputField>();
        wordMiniGame = GetComponentInParent<WordMiniGame>();       
    }

    public void OnSubmit(string eventData)
    {
        EventManager.TriggerEvent(EventType.ChattingActiveOff);

        string text = inputField.text;
        Debug.Log("입력 완료: " + text);

        inputField.text = "";
        inputField.ActivateInputField();

        // 이벤트
        wordMiniGame.wordhandler.Invoke(text);
    }

    // Update is called once per frame  
    void Update()
    {

    }

   
    public void OnEnter(string text)
    {       
        string texts = inputField.text;
        Debug.Log("유저 입력: " + text);
        inputField.text = "";            // 입력 내용 삭제
        inputField.ActivateInputField(); // 포커스 다시 주기
        wordMiniGame.wordhandler.Invoke(texts);       
    }
    WordMiniGame wordMiniGame;
    InputField inputField;
}
