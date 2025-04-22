using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerNameInput : MonoBehaviour
{
    [Header("���â")]
    [SerializeField] private GameObject nicknameIsNull;
    [SerializeField] private GameObject lengthExceeded;

    private int warning=0;
    public TMP_InputField nameInputField;

    private GameObject currentWarning;  // ���â �ν��Ͻ��� ������ ����


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
            {
                Destroy(currentWarning);
                Time.timeScale = 0;
            }     

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
                Debug.Log("�Էµ� �̸�: " + playerName);
                PlayerPrefs.SetString("PlayerName", playerName);
            }
            else
            {
                if(currentWarning==null)
                {
                    currentWarning = Instantiate(lengthExceeded); // ���� ��� = ���� �ʰ�
                }
                warning = 2;
            }
        }
        else
        {
            warning = 1;
            if(currentWarning==null)
            currentWarning = Instantiate(nicknameIsNull); // ���� ��� = �ؽ�Ʈ ��
        }
    }

}
