using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Item : MonoBehaviour
{
    private void Awake()
    {
        itemCount = GetComponentInChildren<TextMeshProUGUI>();
        image = GetComponent<Image>();
        itemCount.text = "1";
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //if(itemCount.text == "0")
        //{
        //    Destroy(gameObject);
        //}
    }

    public void ChangeImage()
    {
        Sprite newSprite = Resources.Load<Sprite>("inventory_Select"); // "Resources/UI/MyIcon.png"

        image.sprite = newSprite;
    }
    public void InitImage()
    {
        Sprite newSprite = Resources.Load<Sprite>("inventory_NoSelect"); // "Resources/UI/MyIcon.png"

        image.sprite = newSprite;
    }
    public void AddItemCount()
    {
        int currentCount = int.Parse(itemCount.text);

        currentCount += 1;

        itemCount.text = currentCount.ToString();
    }
    public void MusItemCount()
    {
        int currentCount = int.Parse(itemCount.text);

        currentCount -= 1;

        itemCount.text = currentCount.ToString();
    }

    public int GetItemCount()
    {
        return int.Parse(itemCount.text);
    }


    TextMeshProUGUI itemCount;

    Image image;
}
