using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;
using TMPro;

public class RoomManager : MonoBehaviourPunCallbacks
{
    [Header("Slot UI")]
    [SerializeField] private RectTransform[] slotPoints; // length=5
    [SerializeField] private GameObject playerSlotPrefab;

    //[Header("Chat UI")]
    //[SerializeField] private InputField chatInput;
    //[SerializeField] private Button sendChatButton;
    //[SerializeField] private TextMeshProUGUI[] chatLines;

    [Header("Ready UI")]
    [SerializeField] private Button readyButton;
    [SerializeField] private Image readyButtonImage;
    [SerializeField] private Sprite readyOffSprite;
    [SerializeField] private Sprite readyOnSprite;

    [Header("Other UI")]
    [SerializeField] private Button leaveButton;

    [Header("Role Assignment")]
    [SerializeField] private AssignManager assignManager;

    private Dictionary<Photon.Realtime.Player, PlayerSlot> slotMap = new Dictionary<Photon.Realtime.Player, PlayerSlot>();
    private bool[] occupied = new bool[5];

    private void Start()
    {
        leaveButton.onClick.AddListener(() => PhotonNetwork.LeaveRoom());
       //sendChatButton.onClick.AddListener(OnClickSendChat);
    }

    public override void OnJoinedRoom()
    {
        SpawnAllSlots();
    }
    public override void OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer)
    {
        SpawnSlot(newPlayer, animate: true);
        //photonView.RPC(nameof(ChatRPC), RpcTarget.All, $"<color=yellow>{newPlayer.NickName}님 입장</color>");
    }
    public override void OnPlayerLeftRoom(Photon.Realtime.Player otherPlayer)
    {
        RemoveSlot(otherPlayer);
        //photonView.RPC(nameof(ChatRPC), RpcTarget.All, $"<color=yellow>{otherPlayer.NickName}님 퇴장</color>");
    }
    public override void OnLeftRoom()
    {
        SceneManager.Instance.LoadSceneAsync(SceneName.RobbyScene);
    }

    public override void OnPlayerPropertiesUpdate(Photon.Realtime.Player target, ExitGames.Client.Photon.Hashtable changedProps)
    {
        if (changedProps.ContainsKey("Ready"))
        {
            slotMap[target].SetReadyState((bool)changedProps["Ready"]);
            TryAssignRoles();
        }
    }

    private void SpawnAllSlots()
    {
        SpawnSlot(PhotonNetwork.LocalPlayer, 2, animate: false);
        var others = PhotonNetwork.PlayerList.Where(p => p != PhotonNetwork.LocalPlayer).ToArray();
        for (int i = 0; i < others.Length; i++)
        {
            int idx = GetNextFreeIndex(i < 2 ? i : i + 1);
            SpawnSlot(others[i], idx, animate: false);
        }
    }
    private void SpawnSlot(Photon.Realtime.Player p, int index = -1, bool animate = false)
    {
        if (slotMap.ContainsKey(p)) return;

        // Instantiate(prefab, parent, worldPositionStays)
        var go = Instantiate(playerSlotPrefab, slotPoints[index], false) as GameObject;
        go.transform.SetAsLastSibling();

        // 초기화
        var slot = go.GetComponent<PlayerSlot>();
        slot.Initialize(p, index);
        slot.SetReadyState(false);

        slotMap[p] = slot;
        occupied[index] = true;

        if (animate) slot.PlayDropAnimation();
    }
    private void RemoveSlot(Photon.Realtime.Player p)
    {
        if (!slotMap.TryGetValue(p, out var slot)) return;
        occupied[slot.SlotIndex] = false;
        Destroy(slot.gameObject);
        slotMap.Remove(p);
    }
    private int GetNextFreeIndex(int start)
    {
        for (int off = 0; off < occupied.Length; off++)
        {
            int i = (start + off) % occupied.Length;
            if (!occupied[i] && i != 2) return i;
        }
        return 2;
    }

    //private void OnClickSendChat()
    //{
    //    if (string.IsNullOrEmpty(chatInput.text)) return;
    //    photonView.RPC(nameof(ChatRPC), RpcTarget.All, $"<b>{PhotonNetwork.NickName}:</b> {chatInput.text}");
    //    chatInput.text = "";
    //}
    [PunRPC]
    //private void ChatRPC(string msg)
    //{
    //    bool placed = false;
    //    for (int i = 0; i < chatLines.Length; i++)
    //    {
    //        if (string.IsNullOrEmpty(chatLines[i].text))
    //        {
    //            chatLines[i].text = msg;
    //            placed = true;
    //            break;
    //        }
    //    }
    //    if (!placed)
    //    {
    //        for (int i = 1; i < chatLines.Length; i++) chatLines[i - 1].text = chatLines[i].text;
    //        chatLines[^1].text = msg;
    //    }
    //}

    private void TryAssignRoles()
    {
        if (!PhotonNetwork.IsMasterClient) return;
        if (PhotonNetwork.CurrentRoom.PlayerCount == 5 &&
            PhotonNetwork.PlayerList.All(p =>
                p.CustomProperties.TryGetValue("Ready", out var v) && (bool)v))
        {
            assignManager.AssignRole();
        }
    }
}