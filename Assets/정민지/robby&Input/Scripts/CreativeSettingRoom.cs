using UnityEngine;
using UnityEngine.UI;

public class CreativeSettingRoom : MonoBehaviour
{
    public GameObject roomItemPrefab; // 프리팹 연결
    public Transform contentParent;   // Scroll View > Content 오브젝트
    private string roompassword;

    public void AddRoom(string roomName, string roomPassword)
    {
        GameObject roomItem = Instantiate(roomItemPrefab, contentParent);
        roompassword = roomPassword;

        var texts = roomItem.GetComponentsInChildren<TMPro.TMP_Text>();
        texts[0].text = name;
    }
}
