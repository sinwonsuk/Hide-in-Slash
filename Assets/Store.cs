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
        // ���⿡ �κ�ũ ����� �ڴµ� �׷����� 

        Debug.Log("Item bought: " + itemName);
    }

    [SerializeField]
    Inventory inventory;



}
