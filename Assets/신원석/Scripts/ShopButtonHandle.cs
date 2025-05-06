using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class ShopButtonHandle : MonoBehaviour
{
    public Button myButton;

    void Start()
    {
        GameObject inventoryObject = GameObject.Find("Inventory");

        inventory = inventoryObject.GetComponentInChildren<Inventory>();

        myButton.onClick.AddListener(() => MyFunction(type));

        price = int.Parse(priceString.text);
    }

    void MyFunction(int message)
    {
        if (Money.instance.GetMoney() < price)
        {
            Debug.Log("Not enough money to buy the item.");
            return;
        }

        SoundManager.GetInstance().SfxPlay(SoundManager.sfx.BuyItem, false);

        Money.instance.MusMoney(price);

        inventory.AddItem(type, price);
    }

    int price = 0;

    [SerializeField]
    int type = 0;

    [SerializeField]
    Inventory inventory;

    [SerializeField]
    TextMeshProUGUI priceString;
}
