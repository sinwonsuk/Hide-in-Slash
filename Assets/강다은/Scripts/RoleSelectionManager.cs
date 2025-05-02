using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using ExitGames.Client.Photon;
using System.Linq;

public class RoleSelectionManager : MonoBehaviourPun
{
    [Header("추격자(UI)와 도망자(UI) 패널")]
    [SerializeField] private GameObject BossPanel;
    [SerializeField] private GameObject RunnerPanel;

    [Header("슬롯 컨테이너")]
    [SerializeField] private Transform runnerSlotContainer;

    [Header("PlayerSlot")]
    [SerializeField] private GameObject profileSlotPrefab;

    [Header("Boss 전용")]
    [SerializeField] private Button[] bossChoiceButtons;
    [SerializeField] private Image[] bossChoiceImages;         
    [SerializeField] private Sprite[] bossSprites;               

    [Header("게임 시작 버튼")]
    [SerializeField] private Button startButton;

    public void Initialize()
    {
        bool isBoss = false;
        if (PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue("Role", out object roleObj)
         && roleObj is string roleName)
        {
            isBoss = NetworkProperties.instance.GetMonsterStates(roleName);
        }

        BossPanel.SetActive(isBoss);
        RunnerPanel.SetActive(!isBoss);

        // RunnerPanel 세팅
        if (!isBoss)
        {
            // 기존 슬롯 모두 삭제
            foreach (Transform ch in runnerSlotContainer) Destroy(ch.gameObject);

            // 내 프로필 슬롯 하나 띄우기
            var go = Instantiate(profileSlotPrefab, runnerSlotContainer);
            var prof = go.GetComponent<OtherPlayerProfile>();
            prof.targetPlayer = PhotonNetwork.LocalPlayer;
            prof.Init();
        }

        // BossPanel 세팅
        if (isBoss)
        {
            // 버튼 이미지 세팅
            for (int i = 0; i < bossChoiceButtons.Length; i++)
            {
                int idx = i;  // 클로저 방지
                bossChoiceImages[i].sprite = bossSprites[i];
                bossChoiceButtons[i].onClick.RemoveAllListeners();
                bossChoiceButtons[i].onClick.AddListener(() =>
                {
                    // 로컬 플레이어 Role 업데이트
                    string chosen = bossSprites[idx].name;
                    PhotonNetwork.LocalPlayer.SetCustomProperties(
                        new Hashtable { { "Role", chosen } }
                    );
                    // 선택한 버튼만 강조
                    HighlightBossChoice(idx);
                });
            }
        }

        // “시작” 버튼 세팅
        startButton.onClick.RemoveAllListeners();
        startButton.onClick.AddListener(() =>
        {
            PhotonNetwork.LocalPlayer.SetCustomProperties(
                new Hashtable { { "Ready", true } }
            );
            startButton.interactable = false;
        });
    }

    private void HighlightBossChoice(int chosenIdx)
    {
        // 예: 선택된 버튼만 강조(비활성화) 처리
        for (int i = 0; i < bossChoiceButtons.Length; i++)
        {
            bossChoiceButtons[i].interactable = (i != chosenIdx);
            bossChoiceImages[i].color = (i == chosenIdx ? Color.white : new Color(1, 1, 1, 0.5f));
        }
    }

}
