using UnityEngine;
using UnityEngine.UI;

public class OtherPlayerProfile : MonoBehaviour
{
    public Photon.Realtime.Player targetPlayer;  // 이 프로필이 해당하는 플레이어
    Image profileImage;  // 프로필 이미지 렌더러

    public Sprite aliveSprite;  // 살아있는 상태 이미지
    public Sprite deadSprite;   // 죽은 상태 이미지
    public Sprite capturedSprite; // 감금 상태 이미지


    private void Start()
    {
        profileImage = GetComponent<Image>();
    }

    // 상태 변경에 따라 UI 업데이트
    public void UpdateProfileState(string state)
    {
        switch (state)
        {
            case "Alive":
                profileImage.sprite = aliveSprite;
                break;
            case "Dead":
                profileImage.sprite = deadSprite;
                break;
            case "Captured":
                profileImage.sprite = capturedSprite;
                break;
            default:
                profileImage.sprite = aliveSprite;
                break;
        }
    }
}