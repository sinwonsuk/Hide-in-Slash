using UnityEngine;
using static SoundManager;
using UnityEngine.Rendering;

public class titleButton : MonoBehaviour
{
    [Header("로그인 창")]
    [SerializeField] private GameObject login;
    [SerializeField] private GameObject settingSound;
    [SerializeField] private GameObject guide;

    [SerializeField] private GameObject Manager;

    void Start()
    {
        Instantiate(Manager);
        guide.SetActive(false);
    }

    public void OnClickStart()
    {
        SoundManager.GetInstance().SfxPlay(SoundManager.sfx.SfxSetting, false);
        Instantiate(login);
    }

    public void OnClickGuide()
    {
        SoundManager.GetInstance().SfxPlay(SoundManager.sfx.SfxSetting, false);
        guide.SetActive(true);
    }

    public void OnClickOption()
    {
        SoundManager.GetInstance().SfxPlay(SoundManager.sfx.SfxSetting, false);
        Instantiate(settingSound);
    }

    public void OnClickExit()
    {
        SoundManager.GetInstance().SfxPlay(SoundManager.sfx.SfxSetting, false);
        Application.Quit();
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            guide.SetActive(false);
        }
    }
}
