using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MemoryButtonClick : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        memoryMiniGame = GetComponentInParent<MemoryMiniGame>();

        spritePaths = new Dictionary<ColorState, string>
        {
            { ColorState.Red, "RedButtonDown" },
            { ColorState.Green, "GreenButtonDown" },
            { ColorState.Blue, "BlueButtonDown" },
            { ColorState.Pink, "PinkButtonDown" },
            { ColorState.Yellow, "YellowButtonDown" }
        };
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (memoryMiniGame.activeButtonAction.Invoke())
        {
            // 색상에 맞는 스프라이트를 딕셔너리에서 로드하여 설정
            if (spritePaths.ContainsKey(color))
            {
                spriteRenderer.sprite = Resources.Load<Sprite>(spritePaths[color]);
            }
        }
    }

    // Pointer Up 이벤트 처리
    public void OnPointerUp(PointerEventData eventData)
    {
        if (memoryMiniGame.activeButtonAction.Invoke())
        {
            memoryMiniGame.checkColorAction.Invoke((int)color);

            // 원래 스프라이트로 복원
            if (spritePaths.ContainsKey(color))
            {
                spriteRenderer.sprite = Resources.Load<Sprite>(spritePaths[color].Replace("Down", ""));
            }
        }
    }

    MemoryMiniGame memoryMiniGame;

    [SerializeField]
    ColorState color;

    SpriteRenderer spriteRenderer;

    private Dictionary<ColorState, string> spritePaths;
}
