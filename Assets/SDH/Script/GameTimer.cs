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

            if (PhotonNetwork.IsMasterClient)
                DeadManager.Instance.OnTimeOut();
        }
    }

    [PunRPC]
    public void RPC_SetStartTime(double time)
    {
        startTime = time;
    }

}