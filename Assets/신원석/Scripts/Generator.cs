using Photon.Pun;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Windows;
using Input = UnityEngine.Input;

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
        {
            if (generatorMiniGame != null)
            {
                Destroy(generatorMiniGame.gameObject);
                generatorMiniGame = null;
            }

            return;
        }
           



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
            // E키를 눌렀을 때 사운드 재생
            if (Input.GetKeyDown(KeyCode.E))
            {
                SoundManager.GetInstance().SfxPlay(SoundManager.sfx.Generator, true);
            }

            if (Input.GetKeyUp(KeyCode.E))
            {
                SoundManager.GetInstance().Sfx_Stop(SoundManager.sfx.Generator);
            }

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
        view = collision.GetComponent<PhotonView>();

        if (view != null && view.IsMine && collision.CompareTag("Player"))
        {            
            isPlayerColilision = true;
        }
   
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player")) return; 

        view = collision.GetComponent<PhotonView>();

        if (view != null && view.IsMine )
        {
            SoundManager.GetInstance().Sfx_Stop(SoundManager.sfx.Generator);
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
                    GameObject miniGame = Instantiate(miniGamePrefab);
                    miniGame.transform.position = transform.position;

                    generatorMiniGame = miniGame.GetComponent<GeneratorMiniGame>();

                    generatorMiniGame.GeneratorGage = generatorGage;

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
        if (isMiniGameFinished)
            return;

        SoundManager.GetInstance().Sfx_Stop(SoundManager.sfx.Generator);

        if (generatorMiniGame != null)
        {
            Destroy(generatorMiniGame.gameObject);
            generatorMiniGame = null;
        }

        photonView.RPC("Sucess", RpcTarget.All);
        StopGeneration();
        isMiniGameFinished = true;
    }
    [PunRPC]
    public void RequestGeneratorComplete()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            DeleteAction?.Invoke();
        }
    }
    [PunRPC]
    public void Sucess()
    {       
        animator.SetTrigger("Sucess");
        isUpdate = false;
        EventManager.TriggerEvent(EventType.GeneratorSuccess);
    }

    [PunRPC]
    public void AddGage(float _gege)
    {
        if (!PhotonNetwork.IsMasterClient)
            return;

        gage += _gege;

        photonView.RPC("BroadCastGage", RpcTarget.AllBuffered, gage);
    }

    [PunRPC]
    public void BroadCastGage(float _gege)
    {
        generatorGage.generatorInImage.fillAmount = _gege;
    }
    bool isUpdate = true;

    bool isMiniGameRunning = false;

    bool isPlayerColilision;

    [SerializeField]
    Canvas canvas;

    [SerializeField]
    GameObject miniGamePrefab;

    [SerializeField]
    generatorGage generatorGage;

    GeneratorMiniGame generatorMiniGame;

    Coroutine coroutine;

    Animator animator;

    public UnityAction DeleteAction;

    bool isMiniGameFinished = false;


    PhotonView view;    

    public float gage { get; set; } = 0;

}
