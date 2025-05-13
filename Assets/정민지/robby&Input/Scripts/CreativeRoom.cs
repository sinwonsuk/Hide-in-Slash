using UnityEngine;

public class CreativeRoom : MonoBehaviour
{
    public GameObject createRoomPopup;
    public GameObject pw;

    public void OnClickCreateRoomButton()
    {
        SoundManager.GetInstance().SfxPlay(SoundManager.sfx.SfxSetting, false);
        createRoomPopup.SetActive(true);
    }

}
