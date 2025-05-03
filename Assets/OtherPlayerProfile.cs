using NUnit.Framework;
using Photon.Pun;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum ProfileState
{
    Player1Profile,
    Player2Profile,
    Player3Profile,
    Player4Profile,
    Player5Profile,
    deadSprite,
    prisonSprite,
    Escape,
    AliveSprite,
}


public class OtherPlayerProfile : MonoBehaviour
{
    public Photon.Realtime.Player targetPlayer;  // 이 프로필이 해당하는 플레이어

    [SerializeField]
    Image profileImage;  // 프로필 이미지 렌더러

    Image bg;

    Dictionary<ProfileState, Sprite> keyValuePairs = new Dictionary<ProfileState, Sprite>();

    [SerializeField]
    List<Sprite> sprites = new List<Sprite>();

    int intname = 0;

    string gameName = string.Empty;

    private void Awake()
    {
        for (int i = 0; i < sprites.Count; i++)
        {
            keyValuePairs.Add((ProfileState)i, sprites[i]);
        }

        bg = GetComponent<Image>();
    }

    private void Start()
    {
        
    }
    public void Init()
    {
        // 보스 


        if (targetPlayer.CustomProperties.TryGetValue("ProfileIndex", out object selfRoleObj))
        {
            if (selfRoleObj is int selfRole)
            {
                intname = selfRole;
            }

        }
        if (keyValuePairs.TryGetValue((ProfileState)intname, out Sprite sprites))
        {
            profileImage.sprite = sprites;

            keyValuePairs.Add(ProfileState.AliveSprite, sprites);

        }

    }
    // 상태 변경에 따라 UI 업데이트
    public void UpdateProfileState(ProfileState _state)
    {
        


        if (keyValuePairs.TryGetValue(_state, out Sprite sprites))
        {

            if (_state == ProfileState.Escape)
            {
                bg.sprite = sprites;
                profileImage.enabled = false;
            }
            else
            {
                profileImage.sprite = sprites;
            }                           
        }
    }
}