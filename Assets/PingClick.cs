using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PingClick : MonoBehaviourPunCallbacks
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        collider2D = GetComponent<PolygonCollider2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0)) // 마우스 왼쪽 클릭
        {
            Vector2 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            if (collider2D.OverlapPoint(mouseWorldPos))
            {
                CreateChat(text);
                transform.parent.gameObject.SetActive(false);
            }
        }
    }

    public void CreateChat(string _text)
    {
        string nickname = PhotonNetwork.NickName;

        photonView.RPC("Craete", RpcTarget.All, _text, nickname);
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

        if (NetworkProperties.instance.GetMonsterStates(name) == false)
        {
            GameObject instantiate = Instantiate(chattingObject, chattingObjectParent);

            ChattingOutput chattingOutput = instantiate.GetComponent<ChattingOutput>();

            string nickname = PhotonNetwork.NickName;

            chattingOutput.textMeshProUGUI.text = senderNickname + " : " + _text;

            Canvas.ForceUpdateCanvases();
            scrollRect.verticalNormalizedPosition = 0f;
        }
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
