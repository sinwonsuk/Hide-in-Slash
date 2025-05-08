using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class ShopButtonRotte : MonoBehaviour
{
    public Button myButton;

    void Start()
    {
        myButton.onClick.AddListener(() => MyFunction());

        price = int.Parse(priceString.text);
    }

    void MyFunction()
    {
        if (Money.instance.GetMoney() < price)
        {
            Debug.Log("Not enough money to buy the item.");
            return;
        }

        SoundManager.GetInstance().SfxPlay(SoundManager.sfx.Rotte, false);

        Money.instance.MusMoney(price);


        float random = Random.Range(0, 101);


        if (random <= 24)
        {
            Money.instance.addMoney(200);
        }
        else if (random <= 99)
        {
            Money.instance.addMoney(50);
        }
        else if(random == 100)
        {
            Money.instance.addMoney(1000);
        }
    }

    int price = 0;

    [SerializeField]
    TextMeshProUGUI priceString;
}
