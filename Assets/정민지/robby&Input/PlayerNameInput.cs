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
            Debug.Log("입력된 이름: " + playerName);

            // 예: 로컬 저장
            PlayerPrefs.SetString("PlayerName", playerName);
        }
    }
}
