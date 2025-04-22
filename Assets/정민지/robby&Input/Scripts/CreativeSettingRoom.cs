using UnityEngine;
using UnityEngine.UI;

public class CreativeSettingRoom : MonoBehaviour
{
    public GameObject roomItemPrefab; // ������ ����
    public Transform contentParent;   // Scroll View > Content ������Ʈ
    private string roompassword;

    public void AddRoom(string roomName, string roomPassword)
    {
        GameObject roomItem = Instantiate(roomItemPrefab, contentParent);
        roompassword = roomPassword;

        var texts = roomItem.GetComponentsInChildren<TMPro.TMP_Text>();
        texts[0].text = name;
    }
}
