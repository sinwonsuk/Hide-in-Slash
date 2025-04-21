using TMPro;
using UnityEngine;

public class PlayerNameInput : MonoBehaviour
{
    public TMP_InputField nameInputField;

    public void SavePlayerName()
    {
        string playerName = nameInputField.text;

        if (!string.IsNullOrEmpty(playerName))
        {
            Debug.Log("�Էµ� �̸�: " + playerName);

            // ��: ���� ����
            PlayerPrefs.SetString("PlayerName", playerName);
        }
    }
}
