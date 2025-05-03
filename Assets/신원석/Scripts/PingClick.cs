using Photon.Pun;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PingClick : MonoBehaviourPunCallbacks, IPointerClickHandler
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
     
    }


    public void CreateChat(string _text)
    {
        Debug.Log("CreateChat 호출됨");


        if (photonView != null)
            Debug.Log("photonView 있음");
        else
            Debug.LogWarning("photonView 없음!");

        string nickname = PhotonNetwork.NickName;

        photonView.RPC("Craete", RpcTarget.All, _text, nickname);
    }
    [PunRPC]
    public void Craete(string _text, string senderNickname)
    {
        Debug.Log($"Craete RPC 호출됨 by {senderNickname} with text: {_text}");

        if (PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue("Role", out object selfRoleObj))
        {
            if (selfRoleObj is string selfRole)
            {
                name = selfRole;
            }
        }

        if (NetworkProperties.instance.GetMonsterStates(name) == false)
        {
            GameObject instantiate = Instantiate(chattingObject, chattingObjectParent);

            ChattingOutput chattingOutput = instantiate.GetComponent<ChattingOutput>();

            chattingOutput.textMeshProUGUI.text = senderNickname + " : " + _text;

            Canvas.ForceUpdateCanvases();
            scrollRect.verticalNormalizedPosition = 0f;
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        CreateChat(text);
        transform.parent.gameObject.SetActive(false);     
    }

    [SerializeField]
    private ScrollRect scrollRect;

    [SerializeField]
    Transform chattingObjectParent;

    [SerializeField]
    GameObject chattingObject;

    [SerializeField]
    string text;
}
