using ExitGames.Client.Photon;
using Photon.Pun;
using System;
using UnityEngine;
using UnityEngine.UI;

public class BossSelectionManager : MonoBehaviourPunCallbacks
{
    [SerializeField] private Image bossImage;
    [SerializeField] private Sprite[] bossSprites;
    [SerializeField] private Button leftButton;
    [SerializeField] private Button rightButton;
    [SerializeField] private Button startButton;

    [SerializeField] private Image profileImage;     // 슬롯 프로필 이미지
    [SerializeField] private Sprite[] profileSprites;

    [Header("Button Visuals")]
    [SerializeField] private Image startButtonImage;
    [SerializeField] private Sprite readyOffSprite;
    [SerializeField] private Sprite readyOnSprite;

    private int currentIndex = 0;
    private bool isConfirmed = false;

    private string[] monsterTypes = { "PeanutGhost", "ProteinGhost", "PukeGirlGhost" };

    private void Start()
    {
        leftButton.onClick.AddListener(() => ChangeSprite(-1));
        rightButton.onClick.AddListener(() => { ChangeSprite(1); Debug.Log("RightButton Clicked!"); });
        startButton.onClick.AddListener(OnConfirm);
        UpdateSprite();
    }

    private void ChangeSprite(int direction)
    {
        if (isConfirmed) return;

        currentIndex += direction;

        if (currentIndex < 0) currentIndex = bossSprites.Length - 1;
        if (currentIndex >= bossSprites.Length) currentIndex = 0;

        UpdateSprite();
    }

    private void UpdateSprite()
    {
        bossImage.sprite = bossSprites[currentIndex];
    }

    private void OnConfirm()
    {
        isConfirmed = true;

        string selectedMonster = monsterTypes[currentIndex];
        Debug.Log($"보스 선택됨: {selectedMonster} (index: {currentIndex})");

        PhotonNetwork.LocalPlayer.SetCustomProperties(new Hashtable {
            { "BossType", monsterTypes[currentIndex] },
            { "RoleConfirmed", true }
        });

        if (PhotonNetwork.IsMasterClient)
        {
            Hashtable roomProps = new Hashtable();
            roomProps["BossType"] = selectedMonster;
            PhotonNetwork.CurrentRoom.SetCustomProperties(roomProps);
            Debug.Log($"BossType 방에 설정됨: {selectedMonster}");
        }

        leftButton.interactable = false;
        rightButton.interactable = false;
        startButton.interactable = false;

        if (startButtonImage != null && readyOnSprite != null)
            startButtonImage.sprite = readyOnSprite;

        gameObject.SetActive(false); // UI 닫기
    }

    public void SetProfileImage(int index)
    {
        if (index >= 0 && index < profileSprites.Length)
            profileImage.sprite = profileSprites[index];
    }

    public void SetBossType(string bossType)
    {
        if (isConfirmed) return;
        int index = Array.IndexOf(monsterTypes, bossType);
        if (index >= 0)
        {
            currentIndex = index;
            UpdateSprite();
        }
    }
}
