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
        string nickname = PhotonNetwork.NickName;

        photonView.RPC("Craete", RpcTarget.All, _text, nickname);
    }
    [PunRPC]
    public void Craete(string _text, string senderNickname)
    {
        StartCoroutine(WaitUntilReadyAndCreate(_text, senderNickname));
    }

    private IEnumerator WaitUntilReadyAndCreate(string _text, string senderNickname)
    {
        // 모든 필드가 null이 아닌지 대기
        yield return new WaitUntil(() =>
            NetworkProperties.instance != null &&
            chattingObject != null &&
            chattingObjectParent != null &&
            scrollRect != null);

        Debug.Log($"[SAFE] Craete 실행 by {senderNickname} : {_text}");

        if (PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue("Role", out object selfRoleObj))
        {
            if (selfRoleObj is string selfRole)
            {
                Playername = selfRole;
            }
        }

        if (NetworkProperties.instance.GetMonsterStates(Playername) == false)
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
        chattingManager.wordhandler.Invoke(text);
    }

    [SerializeField]
    private ScrollRect scrollRect;

    [SerializeField]
    Transform chattingObjectParent;

    [SerializeField]
    GameObject chattingObject;

    [SerializeField]
    string text;

    string Playername;

    [SerializeField]
    ChattingManager chattingManager;
}
