using UnityEngine;
using Photon.Pun;

public class PlayerProfile : MonoBehaviourPun
{
    public SpriteRenderer profileImage;  // 프로필 이미지를 표시할 SpriteRenderer
    public Sprite[] profileSprites;      // 상태에 따른 프로필 이미지들

    // 프로필 상태를 변경하는 메서드
    public void ChangeProfileState(int stateIndex)
    {
        if (photonView.IsMine)  // 내 캐릭터일 때만
        {
            photonView.RPC("SyncProfileState", RpcTarget.Others, stateIndex);  // 다른 플레이어들에게 상태 동기화
        }

        // 프로필 상태 변경
        profileImage.sprite = profileSprites[stateIndex];
    }

    // RPC 메서드: 상태를 다른 플레이어에게 동기화
    [PunRPC]
    public void SyncProfileState(int stateIndex)
    {
        profileImage.sprite = profileSprites[stateIndex];  // 상태에 맞는 이미지로 변경
    }

    // 플레이어 이름을 설정하는 메서드
    public void SetPlayerName(string playerName)
    {
        //playerNameText.text = playerName;
    }
}