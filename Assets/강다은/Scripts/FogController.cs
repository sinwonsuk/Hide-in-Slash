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
    string bossName;
    private IEnumerator Start()
    {
        while(true)
        {
            if(AllPlayersHaveRoles())
            {
                
                if (PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue("Role", out object selfRoleObj))
                {
                    if (selfRoleObj is string selfRole)
                    {
                        bossName = selfRole;
                    }
                }

                // 만약 보스라면 

                if (NetworkProperties.instance.GetMonsterStates(bossName) == true)
                {
                    gameObject.SetActive(false);
                    yield break;
                }

                // 만약 플레이어 라면 

                else if (NetworkProperties.instance.GetMonsterStates(bossName) == false)
                {
                    foreach (var player in PhotonNetwork.PlayerList)
                    {
                        if (player.CustomProperties.TryGetValue("Role", out object monster))
                        {
                            if (monster is string selfRole)
                            {
                                if (NetworkProperties.instance.GetMonsterStatesName(selfRole) == "PukeGirlGhost")
                                {
                                    gameObject.SetActive(true);
                                    fogEffect?.SetFloat("FogAmount", 20.0f);
                                    Debug.Log("Fog ON - 생존자 + 보스");
                                    yield break;
                                }
                            }
                        }
                    }                    
                }

                gameObject.SetActive(false);
                fogEffect?.SetFloat("FogAmount", 0.0f);
                Debug.Log("Fog OFF - 생존자 + 보스");
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
