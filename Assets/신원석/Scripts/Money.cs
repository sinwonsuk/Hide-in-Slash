using TMPro;
using UnityEngine;

public class Money : MonoBehaviour
{
    public static Money instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        
    }

    void Update()
    {
        
    }

    public int GetMoney()
    {
        return int.Parse(moneyCount.text);
    }

    public void addMoney(int amount)
    {
        int money = int.Parse(moneyCount.text);
        money += amount;
        moneyCount.text = money.ToString();
    }
    public void MusMoney(int amount)
    {
        int money = int.Parse(moneyCount.text);
        money -= amount;
        moneyCount.text = money.ToString();
    }

    [SerializeField]
    TextMeshProUGUI moneyCount;

}
