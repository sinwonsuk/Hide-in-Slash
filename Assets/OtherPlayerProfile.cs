using UnityEngine;
using UnityEngine.UI;

public class OtherPlayerProfile : MonoBehaviour
{
    public Photon.Realtime.Player targetPlayer;  // �� �������� �ش��ϴ� �÷��̾�
    Image profileImage;  // ������ �̹��� ������

    public Sprite aliveSprite;  // ����ִ� ���� �̹���
    public Sprite deadSprite;   // ���� ���� �̹���
    public Sprite capturedSprite; // ���� ���� �̹���


    private void Start()
    {
        profileImage = GetComponent<Image>();
    }

    // ���� ���濡 ���� UI ������Ʈ
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