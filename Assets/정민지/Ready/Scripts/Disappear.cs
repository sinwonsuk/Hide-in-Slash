using UnityEngine;

public class Disappear : MonoBehaviour
{
    public void OnClickVisibleObject()
    {
        SoundManager.GetInstance().SfxPlay(SoundManager.sfx.SfxSetting, false);
        gameObject.SetActive(false);
    }
}
