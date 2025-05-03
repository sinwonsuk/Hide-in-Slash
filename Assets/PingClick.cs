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

    void Update()
    {
        //if (Input.GetMouseButtonDown(0))
        //{
        //    // 카메라의 z 값을 마우스의 z 값에 반영
        //    Vector3 mousePos = Input.mousePosition;
        //    mousePos.z = Mathf.Abs(Camera.main.transform.position.z); // 카메라의 z 값

        //    // 월드 좌표로 변환
        //    Vector2 worldPos = Camera.main.ScreenToWorldPoint(mousePos);

        //    // 레이캐스트로 충돌 확인
        //    RaycastHit2D hit = Physics2D.Raycast(worldPos, Vector2.zero);

        //    if (hit.collider != null && hit.collider.CompareTag(tagName))
        //    {
        //        CreateChat(text);
        //        transform.parent.gameObject.SetActive(false);
        //        Debug.Log("2D Object Hit: " + hit.collider.gameObject.name);
        //    }
        //}
    }
    // Update is called once per frame
    //void Update()
    //{


    //    if (Input.GetMouseButtonDown(0)) // 마우스 왼쪽 클릭
    //    {
    //        Vector2 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

    //        if (collider2D.OverlapPoint(mouseWorldPos))
    //        {
    //            CreateChat(text);
    //            transform.parent.gameObject.SetActive(false);
    //        }
    //    }
    //}

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

    new PolygonCollider2D collider2D;

    [SerializeField]
    GameObject chattingObject;

    [SerializeField]
    string text;
}
