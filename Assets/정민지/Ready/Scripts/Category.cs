using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Category : MonoBehaviour
{
    [SerializeField] private Image targetImage;
    [SerializeField] private Sprite MissionSprite;
    [SerializeField] private Sprite GeneratorSprite;
    [SerializeField] private Sprite MiniGameSprite;
    [SerializeField] private Sprite ItemSprite;

    [SerializeField] private GameObject Mission;
    [SerializeField] private GameObject Generartor;
    [SerializeField] private GameObject MiniGame;
    [SerializeField] private GameObject Item;

    public void OnClickMission()
    {
        targetImage.sprite = MissionSprite;
        Mission.SetActive(true);
        Generartor.SetActive(false);
        MiniGame.SetActive(false);
        Item.SetActive(false);
    }

    public void OnClickGenerator()
    {
        targetImage.sprite = GeneratorSprite;
        Mission.SetActive(false);
        Generartor.SetActive(true);
        MiniGame.SetActive(false);
        Item.SetActive(false);
    }

    public void OnClickMiniGame()
    {
        targetImage.sprite = MiniGameSprite;
        Mission.SetActive(false);
        Generartor.SetActive(false);
        MiniGame.SetActive(true);
        Item.SetActive(false);
    }

    public void OnClickItem()
    {
        targetImage.sprite = ItemSprite;
        Mission.SetActive(false);
        Generartor.SetActive(false);
        MiniGame.SetActive(false);
        Item.SetActive(true);
    }

}
