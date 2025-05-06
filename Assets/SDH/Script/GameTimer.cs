using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;
using System.Collections; // Photon 관련 Room Properties용

public class GameTimer : MonoBehaviourPunCallbacks
{
    private float countdownTime = 900f;
    private double startTime;
    private bool isEnded = false;

    public Text timerText;

    private void Start()
    {
        SoundManager.GetInstance().PlayBgm(SoundManager.bgm.Help);

        if (PhotonNetwork.IsMasterClient)
        {
            startTime = PhotonNetwork.Time;
            photonView.RPC("RPC_SetStartTime", RpcTarget.Others, startTime);
        }
    }

    private void Update()
    {
        double elapsed = PhotonNetwork.Time - startTime;
        float timeRemaining = countdownTime - (float)elapsed;

        if (timeRemaining < 0)
            timeRemaining = 0;


        int minutes = Mathf.FloorToInt(timeRemaining / 60);
        int seconds = Mathf.FloorToInt(timeRemaining % 60);
        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);

        if (timeRemaining <= 0 && !isEnded)
        {
            isEnded = true;
            //photonView.RPC("OnTimeoutEnd", RpcTarget.All);

        }
    }

    [PunRPC]
    public void RPC_SetStartTime(double time)
    {
        startTime = time;
    }

    //[PunRPC]
    //void OnTimeoutEnd()
    //{
    //    // 모든 Player 오브젝트 중 내 것만 실행
    //    var players = Object.FindObjectsByType<Player>(FindObjectsSortMode.None);
    //    foreach (var p in players)
    //    {
    //        if (p.photonView.IsMine)
    //        {
    //            p.TriggerDeathByTimeout();
    //            break;
    //        }
    //    }
    //}
}