using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class ProfileSlotManager : MonoBehaviourPunCallbacks
{
    public GameObject profileSlotPrefab;  // ������ ���� ������
    public Transform profileSlotParent;   // ������ ������ �߰��� �θ� ��ü

    public Vector2[] profileTransforms;

    int check = 0;

    void Start()
    {
        // �濡 ������ ��, �ٸ� �÷��̾���� �������� ����
        foreach (var player in PhotonNetwork.PlayerList)
        {
            CreateProfileSlot(player);
        }
    }

    public void CreateProfileSlot(Photon.Realtime.Player targetPlayer)
    {
        // ������ ���� ����
        GameObject slot = Instantiate(profileSlotPrefab, profileSlotParent);

        slot.GetComponent<RectTransform>().anchoredPosition = profileTransforms[check];

        slot.GetComponent<OtherPlayerProfile>().targetPlayer = targetPlayer;

        check++;

        // �����ʿ� ���¸� ����ȭ
        //photonView.RPC("SyncProfileState", RpcTarget.AllBuffered, targetPlayer.UserId, "Alive");  // �ʱ� ���´� 'Alive'�� ����
    }

    // �÷��̾ �濡 ���� �� ������ ������ ����
    public override void OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer)
    {
        CreateProfileSlot(newPlayer);
    }

    // �÷��̾ ���� ���� �� ������ ������ ����
    public override void OnPlayerLeftRoom(Photon.Realtime.Player otherPlayer)
    {
        RemoveProfileSlot(otherPlayer);
    }

    public void RemoveProfileSlot(Photon.Realtime.Player targetPlayer)
    {
        // ������ ���� ���� ���� (���÷� �ּ� ó���� �κ� ��� ����)
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

    // ������ ���� ����ȭ (RPC)
    [PunRPC]
    public void SyncProfileState(Photon.Realtime.Player targetPlayer, string state)
    {
        // playerId�� ���� Ư�� �÷��̾��� ������ ���¸� ������Ʈ
        // ���÷� UI�� ������ ���¸� 'Alive', 'Dead' ������ ������ �� ����
        foreach (Transform child in profileSlotParent)
        {
            OtherPlayerProfile profile = child.GetComponent<OtherPlayerProfile>();
            if (profile != null && profile.targetPlayer == targetPlayer)
            {
                profile.UpdateProfileState(state);  // ���¿� �´� ������ ���� ������Ʈ
            }
        }
    }
}