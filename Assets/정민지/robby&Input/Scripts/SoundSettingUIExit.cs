using UnityEngine;

public class SoundSettingUIExit : MonoBehaviour
{
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)&& gameObject.activeSelf == true)
        {
            SoundManager.GetInstance().SfxPlay(SoundManager.sfx.SfxSetting, false);
            Destroy(gameObject);
        }
    }

    public void OnClickSoundSettingExit()
    {
        SoundManager.GetInstance().SfxPlay(SoundManager.sfx.SfxSetting, false);
        Destroy(gameObject);
    }
}
