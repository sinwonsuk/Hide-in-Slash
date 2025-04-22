using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerNameInput : MonoBehaviour
{
    [Header("경고창")]
    [SerializeField] private GameObject nicknameIsNull;
    [SerializeField] private GameObject lengthExceeded;

    private int warning=0;
    public TMP_InputField nameInputField;

    private GameObject currentWarning;  // 경고창 인스턴스를 저장할 변수


    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
            SavePlayerName();

        if (Input.GetKeyDown(KeyCode.Escape) && warning == 0)
        {
            Time.timeScale = 1;
            Destroy(gameObject);
        }
            

        if ((Input.GetKeyUp(KeyCode.Escape)) && (warning == 1 || warning == 2))
        {
            if (currentWarning != null)
                Destroy(currentWarning);

            warning = 0;
        }
    }

    public void OnClick()
    {
        SavePlayerName();
    }

    public void SavePlayerName()
    {
        string playerName = nameInputField.text;

        if (!string.IsNullOrEmpty(playerName))
        {
            if (playerName.Length < 7)
            {
                Debug.Log("입력된 이름: " + playerName);
                PlayerPrefs.SetString("PlayerName", playerName);
            }
            else
            {
                if(currentWarning==null)
                {
                    currentWarning = Instantiate(lengthExceeded); // 인스턴스 저장!
                }
                warning = 2;
            }
        }
        else
        {
            warning = 1;
            if(currentWarning==null)
            currentWarning = Instantiate(nicknameIsNull); // 인스턴스 저장!
        }
    }

}
