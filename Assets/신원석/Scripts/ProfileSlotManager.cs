using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;

public class ProfileSlotManager : MonoBehaviourPunCallbacks
{
    public GameObject profileSlotPrefab;  // 프로필 슬롯 프리팹

    [Header("플레이어 프로필 부모 위치")]
    public Transform playerProfileSlotParent;   // 프로필 슬롯을 추가할 부모 객체

    [Header("보스 프로필 부모 위치")]
    public Transform bossProfileSlotParent;

    [Header("플레이어 프로필 위치")]
    public Vector2[] playerProfileTransforms;
    [Header("보스 프로필 위치")]
    public Vector2[] bossProfileTransforms;

    string gameName="";
    string profileMonsterName;
    int playerCheck = 0;
    int bossCheck = 0;

    [SerializeField]
    Canvas playerCanvas;

    [SerializeField]
    Canvas bossCanvas;

    private void Init()
    {
        if (PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue("Role", out object selfRoleObj))
        {
            if (selfRoleObj is string selfRole)
            {
                gameName = selfRole;
            }
        }
        // 만약 몬스터라면
        if (NetworkProperties.instance.GetMonsterStates(gameName) == true)
        {
            bossCanvas.gameObject.SetActive(true);
            playerCanvas.gameObject.SetActive(false);
        }
        else
        {
            bossCanvas.gameObject.SetActive(false);
            playerCanvas.gameObject.SetActive(true);
        }

    }

    void Start()
    {       
        StartCoroutine(StartGame());
    }
    IEnumerator StartGame()
    {
        while (true)
        {
            if (AllPlayersHaveRoles())
            {
                Init();
                CreateProfileSlot();            
                 yield break;              
            }
            yield return null;
        }
    }

    private bool AllPlayersHaveRoles()
    {
        foreach (var player in PhotonNetwork.PlayerList)
        {
            if (!player.CustomProperties.ContainsKey("Role"))
            {
                return false;
            }

            if (!player.CustomProperties.ContainsKey("SpawnIndex"))
            {
                return false;
            }
        }
        return true;
    }

    public bool CreateProfileSlot()
    {
      
        if (NetworkProperties.instance.GetMonsterStates(gameName))
        {
            foreach (var player in PhotonNetwork.PlayerList)
            {
                if (player != PhotonNetwork.LocalPlayer)
                {

                    //// 슬롯 생성
                    //GameObject slot = Instantiate(profileSlotPrefab, bossProfileSlotParent);
                    //slot.GetComponent<RectTransform>().anchoredPosition = bossProfileTransforms[playerCheck];
                    //slot.GetComponent<OtherPlayerProfile>().targetPlayer = player;
                    //slot.GetComponent<OtherPlayerProfile>().Init();
                    //playerCheck++;
                }

            }
        }   
        // 내가 플레이어 라면 
        else
        {
         
            foreach (var player in PhotonNetwork.PlayerList)
            {

                if (player == PhotonNetwork.LocalPlayer)
                    continue;


                if (player.CustomProperties.TryGetValue("Role", out object selfRoleObj))
                {
                    if (selfRoleObj is string selfRole)
                    {
                        profileMonsterName = selfRole;

                    }

                }

                if(NetworkProperties.instance.GetMonsterStates(profileMonsterName) ==false)
                {
                    GameObject slot = Instantiate(profileSlotPrefab, playerProfileSlotParent);
                    slot.GetComponent<RectTransform>().anchoredPosition = playerProfileTransforms[playerCheck];
                    slot.GetComponent<OtherPlayerProfile>().targetPlayer = player;
                    slot.GetComponent<OtherPlayerProfile>().Init();
                    playerCheck++;
                }                       
            }
        }

        return true;
 

        // 프로필 슬롯 생성
        

        // 프로필에 상태를 동기화
        //photonView.RPC("SyncProfileState", RpcTarget.AllBuffered, targetPlayer.UserId, "Alive");  // 초기 상태는 'Alive'로 설정
    }

    // 플레이어가 방에 들어올 때 프로필 슬롯을 생성
    public override void OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer)
    {
        if (NetworkProperties.instance.GetMonsterStates(gameName))
        {
            GameObject slot = Instantiate(profileSlotPrefab, bossProfileSlotParent);
            slot.GetComponent<RectTransform>().anchoredPosition = bossProfileTransforms[bossCheck];
            slot.GetComponent<OtherPlayerProfile>().targetPlayer = newPlayer;
            slot.GetComponent<OtherPlayerProfile>().Init();
            bossCheck++;
        }
        else
        {
            GameObject slot = Instantiate(profileSlotPrefab, playerProfileSlotParent);
            slot.GetComponent<RectTransform>().anchoredPosition = playerProfileTransforms[playerCheck];
            slot.GetComponent<OtherPlayerProfile>().targetPlayer = newPlayer;
            slot.GetComponent<OtherPlayerProfile>().Init();
            playerCheck++;
        }
    }



    // 플레이어가 방을 나갈 때 프로필 슬롯을 제거
    public override void OnPlayerLeftRoom(Photon.Realtime.Player otherPlayer)
    {
        
    }

    public void RemoveProfileSlot(Photon.Realtime.Player targetPlayer)
    {
        // 프로필 슬롯 제거 로직 (예시로 주석 처리된 부분 사용 가능)
        foreach (Transform child in playerProfileSlotParent)
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
    public void SyncProfileState(Photon.Realtime.Player targetPlayer, ProfileState state)
    {
        if (NetworkProperties.instance.GetMonsterStates(gameName))
        {
            foreach (Transform child in bossProfileSlotParent)
            {
                OtherPlayerProfile profile = child.GetComponent<OtherPlayerProfile>();
                if (profile != null && profile.targetPlayer == targetPlayer)
                {
                    profile.UpdateProfileState(state);  // 상태에 맞는 프로필 상태 업데이트
                }
            }
        }
        else
        {
            foreach (Transform child in playerProfileSlotParent)
            {
                OtherPlayerProfile profile = child.GetComponent<OtherPlayerProfile>();
                if (profile != null && profile.targetPlayer == targetPlayer)
                {
                    profile.UpdateProfileState(state);  // 상태에 맞는 프로필 상태 업데이트
                }
            }
        }
    }
}