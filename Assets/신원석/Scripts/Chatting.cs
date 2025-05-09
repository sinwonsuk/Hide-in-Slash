using Photon.Pun;
using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class ChattingManager : MonoBehaviourPunCallbacks
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        EventManager.RegisterEvent(EventType.ChattingActiveOff, ActiveOff);
        EventManager.RegisterEvent(EventType.ChattingActiveOn, ActiveOn);
        wordhandler = CreateChat;
    }
    

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Return) && isActiveInputWindow==false)
        {
            isActiveInputWindow = true;
            chattingInputWindow.SetActive(true);
        }
        else if(Input.GetKeyDown(KeyCode.Return) && isActiveInputWindow == true)
        {
            isActiveInputWindow = false;
            chattingInputWindow.SetActive(false);
        }
    }
    void OnDestroy()
    {
        EventManager.UnRegisterEvent(EventType.ChattingActiveOff, ActiveOff);
        EventManager.UnRegisterEvent(EventType.ChattingActiveOn, ActiveOn);
    }
    public void Send()
    {


      
    }
    public void ActiveOff()
    {
        if(gameObject == null)
        {
            Debug.Log("이거왜 널임?");
        }

        gameObject.SetActive(false);
    }
    public void ActiveOn()
    {
        gameObject.SetActive(true);
    }

    public void CreateChat(string _text)
    {
        string nickname = PhotonNetwork.NickName;

        photonView.RPC("Craete", RpcTarget.All,_text, nickname);       
    }
    [PunRPC]
    public void Craete(string _text, string senderNickname)
    {
        if (PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue("Role", out object selfRoleObj))
        {
            if (selfRoleObj is string selfRole)
            {
                name = selfRole;
            }
        }

        if(NetworkProperties.instance.GetMonsterStates(name) ==false)
        {
            GameObject instantiate = Instantiate(chattingObject, transform);

            ChattingOutput chattingOutput = instantiate.GetComponent<ChattingOutput>();

            string nickname = PhotonNetwork.NickName;

            chattingOutput.textMeshProUGUI.text = senderNickname + " : "+  _text;

            Canvas.ForceUpdateCanvases();
            scrollRect.verticalNormalizedPosition = 0f;
        }    
    }

    [SerializeField]
    private ScrollRect scrollRect;

    public Action<string> wordhandler;

    [SerializeField]
    private GameObject chattingObject;

    [SerializeField]
    private GameObject chattingInputWindow;

    bool isActiveInputWindow;
    new string name;
}
