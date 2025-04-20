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
        string text = inputField.text;
        Debug.Log("�Է� �Ϸ�: " + text);

        inputField.text = "";
        inputField.ActivateInputField();

        // �̺�Ʈ
        wordMiniGame.wordhandler.Invoke(text);
    }

    // Update is called once per frame  
    void Update()
    {

    }

   
    public void OnEnter(string text)
    {       
        string texts = inputField.text;
        Debug.Log("���� �Է�: " + text);
        inputField.text = "";            // �Է� ���� ����
        inputField.ActivateInputField(); // ��Ŀ�� �ٽ� �ֱ�
        wordMiniGame.wordhandler.Invoke(texts);       
    }
    WordMiniGame wordMiniGame;
    InputField inputField;
}
