using UnityEngine;

public class Store : MonoBehaviour
{
   
    void Start()
    {
        
    }


    void Update()
    {
        
    }

    public void BuyItem(string itemName)
    {
        // 여기에 인보크 해줘야 겠는데 그럴려면 

        Debug.Log("Item bought: " + itemName);
    }

    [SerializeField]
    Inventory inventory;



}
