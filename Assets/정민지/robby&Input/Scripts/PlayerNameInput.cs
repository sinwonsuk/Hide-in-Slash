using TMPro;
using UnityEngine;

public class PlayerNameInput : MonoBehaviour
{
    [Header("���â")]
    [SerializeField] private GameObject nicknameIsNull;
    [SerializeField] private GameObject lengthExceeded;


    public TMP_InputField nameInputField;

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Return))
        {
            SavePlayerName();
        }
        
    }

    public void SavePlayerName()
    {
        string playerName = nameInputField.text;

        if (!string.IsNullOrEmpty(playerName))
        {
            if (playerName.Length<7)
            {
                Debug.Log("�Էµ� �̸�: " + playerName);

                // ��: ���� ����
                PlayerPrefs.SetString("PlayerName", playerName);
            }
            else
            {
                Instantiate(lengthExceeded);
            }
        }
        else if(string.IsNullOrEmpty(playerName))
        {
            Instantiate(nicknameIsNull);
        }
    }
}
