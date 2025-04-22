using UnityEngine;

public class CreativeRoom : MonoBehaviour
{
    public GameObject createRoomPopup;

    public void OnClickCreateRoomButton()
    {
        createRoomPopup.SetActive(true);
    }
}
