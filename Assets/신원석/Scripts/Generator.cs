using Photon.Pun;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class Generator : MonoBehaviourPunCallbacks
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        DeleteAction = Delete;
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (isUpdate == false)
            return;

        if (generatorMiniGame != null)
        {
            if(generatorMiniGame.IsCheck == true)
            {
                Destroy(generatorMiniGame.gameObject);
                StopGeneration();
            }          
        }

        if (isPlayerColilision ==true)
        {
            if (Input.GetKey(KeyCode.E))
            {
                canvas.gameObject.SetActive(true);

                if (coroutine == null)
                    coroutine = StartCoroutine(GenerateMiniGame());
            }
            else
            {
                canvas.gameObject.SetActive(false);
                StopGeneration();
            }
        }
     
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        PhotonView pv = collision.GetComponent<PhotonView>();

        if (pv != null && pv.IsMine && collision.CompareTag("Player"))
        {            
            isPlayerColilision = true;
        }
   
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        PhotonView pv = collision.GetComponent<PhotonView>();

        if (pv != null && pv.IsMine && collision.CompareTag("Player"))
        {
            isPlayerColilision = false;
            canvas.gameObject.SetActive(false);
        }

    }

    IEnumerator GenerateMiniGame()
    {
        while (true)
        {
            if (!isMiniGameRunning)
            {
                float waitTime = Random.Range(2f, 5f);
                yield return new WaitForSeconds(waitTime);

                if (!isMiniGameRunning)
                {
                    GameObject miniGame = Instantiate(miniGamePrefab, transform);

                    generatorMiniGame = miniGame.GetComponent<GeneratorMiniGame>();

                    isMiniGameRunning = true;

                }
            }
            yield return null; // 매 프레임 체크
        }
    }
    void StopGeneration()
    {
        if (coroutine != null)
        {
            StopCoroutine(coroutine);
            coroutine = null;
        }

        isMiniGameRunning = false;
    }

    void Delete()
    {
        if (!isMiniGameFinished && generatorMiniGame != null)
        {
            Destroy(generatorMiniGame.gameObject);
            generatorMiniGame = null;
            photonView.RPC("Sucess", RpcTarget.All);
            StopGeneration();
            isMiniGameFinished = true;
        }
    }

    [PunRPC]
    public void Sucess()
    {
        isUpdate = false;
        animator.SetTrigger("Sucess");
        EventManager.TriggerEvent(EventType.GeneratorSuccess);
    }

    bool isUpdate = true;

    bool isMiniGameRunning = false;

    bool isPlayerColilision;

    [SerializeField]
    Canvas canvas;

    [SerializeField]
    GameObject miniGamePrefab;

    GeneratorMiniGame generatorMiniGame;

    Coroutine coroutine;

    Animator animator;

    public UnityAction DeleteAction;

    bool isMiniGameFinished = false;
}
