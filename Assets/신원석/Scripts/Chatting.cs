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

    public void Send()
    {


      
    }
    public void ActiveOff()
    {
        gameObject.SetActive(false);
    }
    public void ActiveOn()
    {
        gameObject.SetActive(true);
    }

    public void CreateChat(string _text)
    {
        photonView.RPC("Craete", RpcTarget.All,_text);       
    }
    [PunRPC]
    public void Craete(string _text)
    {
        if (PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue("Role", out object selfRoleObj))
        {
            if (selfRoleObj is string selfRole)
            {
                name = selfRole;
            }
        }

        if(name != "Boss")
        {
            GameObject instantiate = Instantiate(chattingObject, transform);

            ChattingOutput chattingOutput = instantiate.GetComponent<ChattingOutput>();

            chattingOutput.textMeshProUGUI.text = _text;

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

    string name;
}
