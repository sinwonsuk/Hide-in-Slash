using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Realtime;
using System.Collections;
using PHashtable = ExitGames.Client.Photon.Hashtable;
using Photon.Pun;
using Unity.VisualScripting;

public class PlayerSlot : MonoBehaviour
{
    public int SlotIndex { get; private set; }
    private Photon.Realtime.Player owner;

    [Header("UI References")]
    [SerializeField] private Image profileImage;      // 프로필 이미지
    [SerializeField] private TMP_Text nameText;          // 닉네임 텍스트
    [SerializeField] private Image leverImage;        // 손잡이 이미지
    [SerializeField] private Image buttonImage;       // 준비 버튼 이미지
    [SerializeField] private Button readyButton; // 슬롯 안의 준비 버튼

    [Header("Sprites")]
    [SerializeField] private Sprite[] profileSprites;   // 5개 프로필 스프라이트
    [SerializeField] private Sprite offSprite;        // 버튼 X 스프라이트
    [SerializeField] private Sprite onSprite;         // 버튼 O 스프라이트

    [Header("Animation Settings")]
    [SerializeField] private float leverOffAngle = -30f;
    [SerializeField] private float leverOnAngle = 30f;
    [SerializeField] private float tweenDuration = 0.5f;

    public void Initialize(Photon.Realtime.Player p, int index)
    {
        owner = p;
        SlotIndex = index;
        nameText.text = p.NickName;

        // 프로필 스프라이트 세팅
        if (p.CustomProperties.TryGetValue("ProfileIndex", out object idxObj))
        {
            int idx = (int)idxObj;
            if(idx >= 0 && idx < 5)
                profileImage.sprite = profileSprites[idx];
        }

        // 버튼·레버 초기 상태
        buttonImage.sprite = offSprite;
        leverImage.rectTransform.localRotation
            = Quaternion.Euler(0, 0, leverOffAngle);

        // 드롭 애니메이션
        StartCoroutine(DropAnimation(tweenDuration));
    }

    public void PlayDropAnimation()
    {
        StartCoroutine(DropAnimation(tweenDuration));
    }

    private IEnumerator DropAnimation(float duration)
    {
        var rt = GetComponent<RectTransform>();
        Vector2 target = Vector2.zero;
        Vector2 start = target + Vector2.up * 200f;
        float elapsed = 0f;

        rt.anchoredPosition = start;
        while (elapsed < duration)
        {
            float t = elapsed / duration;
            t = 1f - (1f - t) * (1f - t);  // ease-out
            rt.anchoredPosition = Vector2.Lerp(start, target, t);
            elapsed += Time.deltaTime;
            yield return null;
        }
        rt.anchoredPosition = target;
    }

    public void SetReadyState(bool ready)
    {
        buttonImage.sprite = ready ? onSprite : offSprite;
        StartCoroutine(RotateLever(ready, tweenDuration));
    }

    private IEnumerator RotateLever(bool on, float duration)
    {
        var rt = leverImage.rectTransform;
        float fromZ = rt.localEulerAngles.z;
        float toZ = on ? leverOnAngle : leverOffAngle;
        float elapsed = 0f;

        if (Mathf.Abs(toZ - fromZ) > 180f)
            fromZ += (fromZ < toZ ? 360f : -360f);

        while (elapsed < duration)
        {
            float t = elapsed / duration;
            t = t * t * (3f - 2f * t);  // smoothstep
            float z = Mathf.Lerp(fromZ, toZ, t);
            rt.localRotation = Quaternion.Euler(0, 0, z);
            elapsed += Time.deltaTime;
            yield return null;
        }
        rt.localRotation = Quaternion.Euler(0, 0, toZ);
    }
    public void BindReadyButton()
    {
        // 슬롯 내부 준비 버튼 활성화
        readyButton.gameObject.SetActive(true);
        readyButton.onClick.RemoveAllListeners();
        readyButton.onClick.AddListener(OnClickToggleReady);
    }

    private void OnClickToggleReady()
    {
        // 로컬 플레이어 커스텀 프로퍼티 읽기
        bool curr = PhotonNetwork.LocalPlayer.CustomProperties
                        .TryGetValue("Ready", out var v) && (bool)v;
        bool next = !curr;

        // 서버에 준비 상태 전송
        var hash = new PHashtable { { "Ready", next } };
        owner.SetCustomProperties(hash);

        // 즉시 UI 반영
        SetReadyState(next);
    }
}
