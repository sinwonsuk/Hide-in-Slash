using UnityEngine;
using Photon.Pun;

public class PlayerProfile : MonoBehaviourPun
{
    public SpriteRenderer profileImage;  // ������ �̹����� ǥ���� SpriteRenderer
    public Sprite[] profileSprites;      // ���¿� ���� ������ �̹�����

    // ������ ���¸� �����ϴ� �޼���
    public void ChangeProfileState(int stateIndex)
    {
        if (photonView.IsMine)  // �� ĳ������ ����
        {
            photonView.RPC("SyncProfileState", RpcTarget.Others, stateIndex);  // �ٸ� �÷��̾�鿡�� ���� ����ȭ
        }

        // ������ ���� ����
        profileImage.sprite = profileSprites[stateIndex];
    }

    // RPC �޼���: ���¸� �ٸ� �÷��̾�� ����ȭ
    [PunRPC]
    public void SyncProfileState(int stateIndex)
    {
        profileImage.sprite = profileSprites[stateIndex];  // ���¿� �´� �̹����� ����
    }

    // �÷��̾� �̸��� �����ϴ� �޼���
    public void SetPlayerName(string playerName)
    {
        //playerNameText.text = playerName;
    }
}