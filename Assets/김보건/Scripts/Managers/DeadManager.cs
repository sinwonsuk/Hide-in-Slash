using Photon.Pun;
using System.Collections;
using UnityEngine;

public class DeadManager : MonoBehaviourPun
{
    public static DeadManager Instance;

    public GameObject allDeadUI;
    public GameObject playerDeadUI;

    private int initialPlayerCount = 0;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            int total = PhotonNetwork.PlayerList.Length;
            initialPlayerCount = total - 1;
            photonView.RPC("SetInitialPlayerCount", RpcTarget.All, initialPlayerCount);
        }
    }

    [PunRPC]
    public void SetInitialPlayerCount(int count)
    {
        initialPlayerCount = count;
    }

    public void CheckAllPlayerDead()
    {
        int deadCount = GameObject.FindGameObjectsWithTag("DeadPlayer").Length;

        if (initialPlayerCount < 0)
        {
            return;
        }

        if (deadCount >= initialPlayerCount)
            photonView.RPC("ShowAllDeathUI", RpcTarget.All);
        else
            photonView.RPC("ShowPlayerDeathUI", RpcTarget.All);
    }

    [PunRPC]
    public void ShowAllDeathUI()
    {
        if (allDeadUI != null)
        {
            allDeadUI.transform.SetParent(null);
            DontDestroyOnLoad(allDeadUI);
            allDeadUI.SetActive(true);

            Transform black = allDeadUI.transform.Find("Black");
            if (black != null)
            {
                var fadeEffect = black.GetComponent<playerDeath>();
                if (fadeEffect != null)
                    fadeEffect.TriggerFade();
            }

        }
    }

    [PunRPC]
    public void ShowPlayerDeathUI()
    {
        if (playerDeadUI != null)
        {
            playerDeadUI.transform.SetParent(null);
            DontDestroyOnLoad(playerDeadUI);
            playerDeadUI.SetActive(true);

            Transform black = playerDeadUI.transform.Find("Black");
            if (black != null)
            {
                var fadeEffect = black.GetComponent<playerDeath>();
                if (fadeEffect != null)
                    fadeEffect.TriggerFade();
            }

        }
    }

    private IEnumerator RemoveUIAndMoveScene(GameObject ui, float delay)
    {
        yield return new WaitForSeconds(delay);

        if (ui != null)
        {
            Destroy(ui);
        }

        if (PhotonNetwork.IsConnected && PhotonNetwork.InRoom && PhotonNetwork.LocalPlayer != null)
        {
            // 로컬 클라이언트만 씬 이동
            if (PhotonNetwork.LocalPlayer.IsLocal)
            {
                PhotonNetwork.LoadLevel("RobbyScene");
            }
        }
    }
}
