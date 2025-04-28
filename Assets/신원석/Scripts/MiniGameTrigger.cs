using Photon.Pun;
using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class MiniGameTrigger : MonoBehaviourPunCallbacks
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        closeMiniGameAction = CloseMiniGame;



    }

    // Update is called once per frame
    void Update()
    {
        if (isPlayerInRange && Input.GetKeyDown(KeyCode.E) && isPlaying ==false)
        {
            isPlaying = true;
            OpenMiniGame();
        }
        else if (isPlayerInRange == false && Input.GetKeyDown(KeyCode.E))
        {
            CloseMiniGame();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isPlayerInRange = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
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
            PhotonNetwork.Destroy(gameObject);
            Destroy(miniGameManager.gameObject);
        }
    }
    bool isPlaying = false;
    bool isPlayerInRange = false;

    [SerializeField]
    GameObject miniGameManager;

    UnityAction closeMiniGameAction;
}

