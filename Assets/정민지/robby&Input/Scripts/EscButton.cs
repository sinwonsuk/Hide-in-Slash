using UnityEngine;

public class EscButton : MonoBehaviour
{
    public void OnClick()
    {
        SoundManager.GetInstance().SfxPlay(SoundManager.sfx.SfxSetting, false);
        Destroy(gameObject);
    }
}
