using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using ExitGames.Client.Photon;
using System.Collections.Generic;

public class RoleSelectionManager : MonoBehaviourPunCallbacks
{
    [SerializeField] private Button startButton;
    [SerializeField] private Image profileImage;     // 슬롯 프로필 이미지
    [SerializeField] private Image characterImage;   // 캐릭터 아바타 이미지
    [SerializeField] private Sprite[] profileSprites;
    [SerializeField] private Sprite[] characterSprites;
    [SerializeField] private string[] characterKeys;

    [Header("Button Visuals")]
    [SerializeField] private Image startButtonImage;
    [SerializeField] private Sprite readyOffSprite;
    [SerializeField] private Sprite readyOnSprite;

    private bool isConfirmed = false;

    private readonly List<string> pTypes = new List<string>
    {
        "Player", "Player2", "Player3", "Player4", "Player5", "Player6", "Player7"
    };

    public void SetProfileInfo(int profileIndex, string characterType)
    {
        // 프로필 슬롯 이미지
        if (profileIndex >= 0 && profileIndex < profileSprites.Length)
            profileImage.sprite = profileSprites[profileIndex];

        // 캐릭터 아바타 이미지
        int charIndex = pTypes.IndexOf(characterType);
        if (charIndex >= 0 && charIndex < characterSprites.Length)
            characterImage.sprite = characterSprites[charIndex];
    }

    private void Start()
    {
        startButton.onClick.AddListener(OnClickStart);
    }

    private void OnClickStart()
    {
        if (isConfirmed) return;

        isConfirmed = true;

        PhotonNetwork.LocalPlayer.SetCustomProperties(new Hashtable {
            { "RoleConfirmed", true }
        });

        startButton.interactable = false;

        if (startButtonImage != null && readyOnSprite != null)
            startButtonImage.sprite = readyOnSprite;

        gameObject.SetActive(false);
    }
}
