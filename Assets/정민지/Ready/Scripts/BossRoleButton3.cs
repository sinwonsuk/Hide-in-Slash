using UnityEngine;

public class BossRoleButton3 : MonoBehaviour
{
    [SerializeField] private GameObject Peanut;
    [SerializeField] private GameObject Mamarote;
    [SerializeField] private GameObject Cancer;

    public void OnClickPeanut()
    {
        SoundManager.GetInstance().SfxPlay(SoundManager.sfx.SfxSetting, false);
        Peanut.SetActive(true);
        Mamarote.SetActive(false);
        Cancer.SetActive(false);
    }

    public void OnClickMamarote()
    {
        SoundManager.GetInstance().SfxPlay(SoundManager.sfx.SfxSetting, false);
        Peanut.SetActive(false);
        Mamarote.SetActive(true);
        Cancer.SetActive(false);
    }

    public void OnClickCancer()
    {
        SoundManager.GetInstance().SfxPlay(SoundManager.sfx.SfxSetting, false);
        Peanut.SetActive(false);
        Mamarote.SetActive(false);
        Cancer.SetActive(true);
    }
}
