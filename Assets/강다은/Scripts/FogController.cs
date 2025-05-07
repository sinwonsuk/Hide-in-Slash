using UnityEngine;
using UnityEngine.VFX;
using Photon.Pun;
using System.Collections;

public class FogController : MonoBehaviour
{
    [SerializeField] private VisualEffect fogEffect;
    private float fogAmount = 0f;
    private float maxFogAmount = 1000f;
    private float fogIncreaseStep = 50f;

    private IEnumerator Start()
    {
        while(true)
        {
            if(AllPlayersHaveRoles())
            {

                string bossType = PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue("Role", out object bossTypeObj)
                    ? bossTypeObj as string
                    : "";


                if (bossType == "PukeGirlGhost")
                {
                    gameObject.SetActive(true);
                    fogEffect?.SetFloat("FogAmount", 20f);
                    Debug.Log("Fog ON - 생존자 + PukeGirl 보스");
                }
                else
                {
                    gameObject.SetActive(false);
                    Debug.Log("Fog OFF - 보스가 PukeGirl 아님");
                }

                yield break;
            }
            yield return null;
        }
    }

    public void IncreaseFog()
    {
        if (!gameObject.activeSelf) return;

        fogAmount = Mathf.Clamp(fogAmount + fogIncreaseStep, 0f, maxFogAmount);
        fogEffect?.SetFloat("FogAmount", fogAmount);
        Debug.Log($"Fog 증가 → {fogAmount}");
    }

    private bool AllPlayersHaveRoles()
    {
        foreach (var player in PhotonNetwork.PlayerList)
        {
            if (!player.CustomProperties.ContainsKey("Role"))
            {
                return false;
            }
        }
        return true;
    }
}
