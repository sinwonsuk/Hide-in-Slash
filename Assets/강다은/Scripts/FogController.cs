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
        float timeout = 3f;
        while ((!PhotonNetwork.LocalPlayer.CustomProperties.ContainsKey("isBoss") ||
                !PhotonNetwork.CurrentRoom.CustomProperties.ContainsKey("BossType")) && timeout > 0f)
        {
            timeout -= Time.deltaTime;
            yield return null;
        }

        bool isBoss = PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue("isBoss", out object isBossObj) && (bool)isBossObj;
        string bossType = PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue("BossType", out object bossTypeObj)
            ? bossTypeObj as string
            : "";

        if (isBoss)
        {
            gameObject.SetActive(false);
            Debug.Log("Fog OFF - 나는 보스");
            yield break;
        }

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
    }

    public void IncreaseFog()
    {
        if (!gameObject.activeSelf) return;

        fogAmount = Mathf.Clamp(fogAmount + fogIncreaseStep, 0f, maxFogAmount);
        fogEffect?.SetFloat("FogAmount", fogAmount);
        Debug.Log($"Fog 증가 → {fogAmount}");
    }
}
