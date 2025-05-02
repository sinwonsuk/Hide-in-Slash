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
    [SerializeField] private Transform slotContainer;

    [Header("PlayerSlot")]
    [SerializeField] private GameObject profileSlotPrefab;

    [Header("Boss 전용")]
    [SerializeField] private Button bossChoiceButtons;         
    [SerializeField] private Image[] bossChoiceImages;         
    [SerializeField] private Sprite[] bossSprites;               

    [Header("게임 시작 버튼")]
    [SerializeField] private Button startButton;

    public void Initialize()
    {
        bool isBoss = PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue("Role", out var roleObj)
                      && NetworkProperties.instance.GetMonsterStates((string)roleObj);
        BossPanel.SetActive(isBoss);
        RunnerPanel.SetActive(!isBoss);

        foreach (Transform child in slotContainer)
            Destroy(child.gameObject);

        var me = PhotonNetwork.LocalPlayer;
        var go = Instantiate(profileSlotPrefab, slotContainer);
        var prof = go.GetComponent<OtherPlayerProfile>();
        prof.targetPlayer = me;
        prof.Init();

        startButton.onClick.RemoveAllListeners();
        startButton.onClick.AddListener(() =>
        {
            PhotonNetwork.LocalPlayer.SetCustomProperties(
                new ExitGames.Client.Photon.Hashtable { { "Ready", true } }
            );
            startButton.interactable = false;
        });
    }

}
