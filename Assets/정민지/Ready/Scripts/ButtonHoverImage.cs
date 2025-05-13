using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ButtonHoverImage : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Image targetImage;
    public Sprite normalSprite;
    public Sprite hoverSprite;

    public void OnPointerEnter(PointerEventData eventData)
    {
        targetImage.sprite = hoverSprite;
        ResizeImage(250, 180);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        targetImage.sprite = normalSprite;
        ResizeImage(130, 180);
    }

    void ResizeImage(float width, float height)
    {
        if (targetImage != null)
        {
            RectTransform rt = targetImage.GetComponent<RectTransform>();
            rt.sizeDelta = new Vector2(width, height);
        }
    }
}
