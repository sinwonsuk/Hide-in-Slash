using UnityEngine;

public class NextPage : MonoBehaviour
{
    [SerializeField] private GameObject NewPage;
    [SerializeField] private GameObject CurrentPage;

    public void OnClickNext()
    {
        SoundManager.GetInstance().SfxPlay(SoundManager.sfx.SfxSetting, false);
        CurrentPage.SetActive(false);
        NewPage.SetActive(true);
    }
}
