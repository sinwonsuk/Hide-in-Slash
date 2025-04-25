using UnityEngine;

public class CreativeRoom : MonoBehaviour
{
    public GameObject createRoomPopup;
    public GameObject pw;

    public void OnClickCreateRoomButton()
    {
        createRoomPopup.SetActive(true);
    }

    public void OnClickEnterRoom()
    {
        Instantiate(pw);
    }

}
