using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

public class ChattingInput : MonoBehaviour
{
    private void Awake()
    {
        
    }

    private void OnEnable()
    {
        inputField = GetComponent<InputField>();
        inputField.ActivateInputField();
    }
    
    void Start()
    {
    }

    void Update()
    {
    }

    public void OnEnter(string text)
    {
        string texts = inputField.text;
        Debug.Log("유저 입력: " + text);
        inputField.text = "";
        inputField.ActivateInputField();
        chattingManager.wordhandler.Invoke(text);
    }

    [PunRPC]
    public void Send(string text)
    {
        chattingManager.wordhandler.Invoke(text);
    }

    InputField inputField;
    [SerializeField]
    ChattingManager chattingManager;
}
