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


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        closeMiniGameAction = CloseMiniGame;

    }

    // Update is called once per frame
    void Update()
    {

        for (int i = 0; i < lists.Count; i++)
        {
            if (lists[i].gameObject.GetComponent<Player>().InputE() ==true)
            {
                view = lists[i];

                if (isPlayerInRange && isPlaying == false)
                {
                    photonView.RPC("RequestOpenMiniGame", RpcTarget.MasterClient); // 모두에게 누가 열었는지 전달
                    OpenMiniGame();
                }
            }

        }
       
        //else if (isPlayerInRange == false && Input.GetKeyDown(KeyCode.E))
        //{
        //    CloseMiniGame();
        //}
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
        PhotonView pv = collision.GetComponent<PhotonView>();

        lists.Add(pv);

        if (pv != null && pv.IsMine && collision.CompareTag("Player"))
        {
            if (view == null) 
            {
                view = pv;
            }

            isPlayerInRange = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if(collision.GetComponent<PhotonView>() == view && collision.CompareTag("Player"))
        {
            isPlayerInRange = false;
            CloseMiniGame();
        }
    }

    private void OpenMiniGame()
    {
        if (miniGameManager != null)
        {
            miniGameManager = Instantiate(miniGameManager);

            EventManager.TriggerEvent(EventType.LightOff);
            GameObject choice = miniGameManager.GetComponent<MiniGameManager>().choiceMiniGame;
            MiniGame MiniGame = choice.GetComponentInChildren<MiniGame>();
            MiniGame.trigerAction = closeMiniGameAction;
        }
    }

    private void CloseMiniGame()
    {
        if (miniGameManager != null && isPlaying == true)
        {
            EventManager.TriggerEvent(EventType.LightOn);
            RequestDestroy();        
        }
    }

    public void RequestDestroy()
    {
        if (isDestroyed) return;
        isDestroyed = true;

        if(miniGameManager != null)
        Destroy(miniGameManager.gameObject);

        if (PhotonNetwork.IsMasterClient)
        {
            // 마스터는 직접 제거 가능
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
    bool isPlayerInRange = false;

    [SerializeField]
    GameObject miniGameManager;

    UnityAction closeMiniGameAction;
}

