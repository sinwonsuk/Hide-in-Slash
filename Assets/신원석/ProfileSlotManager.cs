using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class ProfileSlotManager : MonoBehaviourPunCallbacks
{
    public GameObject profileSlotPrefab;  // 프로필 슬롯 프리팹
    public Transform profileSlotParent;   // 프로필 슬롯을 추가할 부모 객체

    public Vector2[] profileTransforms;

    int check = 0;

    void Start()
    {
        // 방에 입장한 후, 다른 플레이어들의 프로필을 생성
        foreach (var player in PhotonNetwork.PlayerList)
        {
            CreateProfileSlot(player);
        }
    }

    public void CreateProfileSlot(Photon.Realtime.Player targetPlayer)
    {
        // 프로필 슬롯 생성
        GameObject slot = Instantiate(profileSlotPrefab, profileSlotParent);

        slot.GetComponent<RectTransform>().anchoredPosition = profileTransforms[check];

        slot.GetComponent<OtherPlayerProfile>().targetPlayer = targetPlayer;

        check++;

        // 프로필에 상태를 동기화
        //photonView.RPC("SyncProfileState", RpcTarget.AllBuffered, targetPlayer.UserId, "Alive");  // 초기 상태는 'Alive'로 설정
    }

    // 플레이어가 방에 들어올 때 프로필 슬롯을 생성
    public override void OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer)
    {
        if (newPlayer.CustomProperties.TryGetValue("Role", out object roleObj))
        {
            string role = roleObj.ToString();
            if (newPlayer != PhotonNetwork.LocalPlayer && role != "Monster")
            {
                CreateProfileSlot(newPlayer);
            }
        }
    }

    // 플레이어가 방을 나갈 때 프로필 슬롯을 제거
    public override void OnPlayerLeftRoom(Photon.Realtime.Player otherPlayer)
    {
        if (otherPlayer.CustomProperties.TryGetValue("Role", out object roleObj))
        {
            string role = roleObj.ToString();
            if (otherPlayer != PhotonNetwork.LocalPlayer && role != "Monster")
            {
                RemoveProfileSlot(otherPlayer);
            }
        }
    }

    public void RemoveProfileSlot(Photon.Realtime.Player targetPlayer)
    {
        // 프로필 슬롯 제거 로직 (예시로 주석 처리된 부분 사용 가능)
        foreach (Transform child in profileSlotParent)
        {
            OtherPlayerProfile profile = child.GetComponent<OtherPlayerProfile>();
            if (profile != null && profile.targetPlayer == targetPlayer)
            {
                Destroy(child.gameObject);
                break;
            }
        }
    }

    // 프로필 상태 동기화 (RPC)
    [PunRPC]
    public void SyncProfileState(Photon.Realtime.Player targetPlayer, string state)
    {
        // playerId를 통해 특정 플레이어의 프로필 상태를 업데이트
        // 예시로 UI의 프로필 상태를 'Alive', 'Dead' 등으로 변경할 수 있음
        foreach (Transform child in profileSlotParent)
        {
            OtherPlayerProfile profile = child.GetComponent<OtherPlayerProfile>();
            if (profile != null && profile.targetPlayer == targetPlayer)
            {
                profile.UpdateProfileState(state);  // 상태에 맞는 프로필 상태 업데이트
            }
        }
    }
}