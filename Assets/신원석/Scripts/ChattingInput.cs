using UnityEngine;
using UnityEngine.UI;

public class ChattingInput : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    private void OnEnable()
    {
        inputField = GetComponent<InputField>();
        inputField.ActivateInputField();
    }

    void Start()
    {
      
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void OnEnter(string text)
    {
        //서버한테 여기서 보내주고 

        string texts = inputField.text;
        Debug.Log("유저 입력: " + text);
        inputField.text = "";            // 입력 내용 삭제
        inputField.ActivateInputField(); // 포커스 다시 주기
        chattingManager.wordhandler.Invoke(texts);
    }
    InputField inputField;
    [SerializeField]
    ChattingManager chattingManager;
}
