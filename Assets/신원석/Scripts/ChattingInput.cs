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
        //�������� ���⼭ �����ְ� 

        string texts = inputField.text;
        Debug.Log("���� �Է�: " + text);
        inputField.text = "";            // �Է� ���� ����
        inputField.ActivateInputField(); // ��Ŀ�� �ٽ� �ֱ�
        chattingManager.wordhandler.Invoke(texts);
    }
    InputField inputField;
    [SerializeField]
    ChattingManager chattingManager;
}
