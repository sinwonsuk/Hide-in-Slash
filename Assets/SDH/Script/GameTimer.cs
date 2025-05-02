using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;
using System.Collections; // Photon 관련 Room Properties용

public class GameTimer : MonoBehaviourPunCallbacks
{
    private float countdownTime = 600f;
    private double startTime;

    public Text timerText;

    private void Start()
    {
        if(PhotonNetwork.IsMasterClient)
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

        if (timeRemaining <= 0)
        {
            Debug.Log("타이머 종료!");
        }
    }

    [PunRPC]
    public void RPC_SetStartTime(double time)
    {
        startTime = time;
    }
}