using Photon.Pun;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class MiniGameTrigger : MonoBehaviourPunCallbacks
{
    PhotonView view;

    bool isDestroyed = false;

    List<PhotonView> lists = new List<PhotonView>();

    GameObject miniGameManagerInstance; 
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        closeMiniGameAction = CloseMiniGame;
    }


    public void TryOpenMiniGame(PhotonView playerView)
    {
        if (isPlaying ==true) 
            return;

        if(playerView.gameObject.tag == "Player")
        {
            SoundManager.GetInstance().SfxPlay(SoundManager.sfx.Click, false); // 사운드 재생

            view = playerView;
            photonView.RPC("RequestOpenMiniGame", RpcTarget.MasterClient);
            OpenMiniGame();
        }       
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    [PunRPC]
    void RequestOpenMiniGame()
    {
        if (PhotonNetwork.IsMasterClient && isPlaying == false)
        {
            isPlaying = true;
            photonView.RPC("ControllMiniGame", RpcTarget.All); // 모두에게 누가 열었는지 전달
        }
    }

    [PunRPC]
    void ControllMiniGame()
    {
        isPlaying = true;      
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
       
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        PhotonView exitView = collision.GetComponent<PhotonView>();

        if (exitView == null || !collision.CompareTag("Player"))
            return;

        // 내가 관리하던 플레이어인지 확인
        if (view != null && exitView.ViewID == view.ViewID)
        {
            if (view.IsMine) // 로컬 플레이어가 나가는 경우에만 미니게임 종료 시도
            {
                isPlayerInRange = false;
                CloseMiniGame();
            }
        }
    }

    public void OpenMiniGame()
    {
        if (miniGameManager != null)
        {
            miniGameManagerInstance = Instantiate(miniGameManager);

//            EventManager.TriggerEvent(EventType.LightOff);
            GameObject choice = miniGameManagerInstance.GetComponent<MiniGameManager>().choiceMiniGame;
            MiniGame MiniGame = choice.GetComponentInChildren<MiniGame>();
            MiniGame.trigerAction = closeMiniGameAction;
        }
    }

    private void CloseMiniGame()
    {
        if (miniGameManagerInstance != null && isPlaying == true)
        {
            EventManager.TriggerEvent(EventType.ChattingActiveOn);
            RequestDestroy();        
        }
    }

    public void RequestDestroy()
    {
        if (isDestroyed) return;
        isDestroyed = true;

        if(miniGameManagerInstance != null)
        Destroy(miniGameManagerInstance.gameObject);

        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.Destroy(gameObject);
        }

        if (!PhotonNetwork.IsMasterClient)
        {
            photonView.RPC("DestroySelf", RpcTarget.MasterClient, photonView.ViewID);
        }
    }
    [PunRPC]
    public void DestroySelf(int viewID)
    {
        PhotonView target = PhotonView.Find(viewID);
        if (target != null)
        {
            PhotonNetwork.Destroy(target.gameObject);
        }
    }


    bool isPlaying = false;
    public bool isPlayerInRange { get; set; } = false;

    [SerializeField]
    GameObject miniGameManager;

    UnityAction closeMiniGameAction;
}

