using UnityEngine;

public class selectedRole : MonoBehaviour
{
    [SerializeField] private GameObject playerRole;
    [SerializeField] private GameObject bossRole;

    public void OnClickPlayerRole()
    {
        SoundManager.GetInstance().SfxPlay(SoundManager.sfx.SfxSetting, false);
        playerRole.SetActive(true);
        bossRole.SetActive(false);
    }

    public void OnClickBossRole()
    {
        SoundManager.GetInstance().SfxPlay(SoundManager.sfx.SfxSetting, false);
        bossRole.SetActive(true);
        playerRole.SetActive(false);
    }


}
