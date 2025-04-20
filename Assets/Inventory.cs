
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public enum InventoryType
{
    Invisibility,
    Tunnel,
    Flashlight,
    EngeryDrink,
    PrisonKey,
}

public class Inventory : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created  
    void Start()
    {
        orderedKeys = new List<InventoryType>();
    }

    // Update is called once per frame  
    void Update()
    {
        MoveInvenSelect();
        MoveSelectRender();
        UseItem();
        //DeleteItem();
    }

    public void MoveSelectRender()
    {
        if (orderedKeys == null || orderedKeys.Count == 0)
            return;

        InventoryType selectedKey = orderedKeys[inventoryMoveIndex];

        foreach (var _item in ItemDictionary)
        {
            Item item = _item.Value.GetComponent<Item>();

            if (_item.Key == selectedKey)
            {
                item.ChangeImage();
            }
            else
            {
                item.InitImage();
            }

        }

    }

    public void MoveInvenSelect()
    {
        if (orderedKeys == null || orderedKeys.Count == 0)
            return;

        if (Input.GetKeyDown(KeyCode.Q))
        {
            if (inventoryMoveIndex <= 0)
            {
                inventoryMoveIndex = 0;
                return;
            }

            inventoryMoveIndex -= 1;
        }
        else if (Input.GetKeyDown(KeyCode.E))
        {
            if (inventoryMoveIndex >= ItemDictionary.Count - 1)
            {
                inventoryMoveIndex = ItemDictionary.Count - 1;
                return;
            }

            inventoryMoveIndex += 1;


        }

    }


    public void AddItem(int _type)
    {
        if (ItemDictionary.TryGetValue((InventoryType)_type, out GameObject gameObject))
        {
            gameObject.GetComponent<Item>().AddItemCount();
        }
        else
        {
            GameObject item = Instantiate(inventoryList[_type], transform);
            ItemDictionary.Add((InventoryType)_type, item);
            orderedKeys.Add((InventoryType)_type);
        }

    }

    public void UseItem()
    {
        if(Input.GetKeyDown(KeyCode.Space) && ItemDictionary.Count > 0)
        {
            TryUseSelectedItem();
        }      
    }


    private void TryUseSelectedItem()
    {
        InventoryType key = orderedKeys[inventoryMoveIndex];

        if (ItemDictionary.TryGetValue(key, out GameObject go))
        {
            Item item = go.GetComponent<Item>();
            item.MusItemCount();

            if(item.GetItemCount() == 0)
            {
                DeleteItem(go);
            }   
        }
    }


    public void DeleteItem(GameObject gameObject)
    {
        if (gameObject.GetComponent<Item>().GetItemCount() == 0)
        {
            Destroy(gameObject);
            ItemDictionary.Remove(orderedKeys[inventoryMoveIndex]);
            orderedKeys.Remove(orderedKeys[inventoryMoveIndex]);   
        }

        if (inventoryMoveIndex == 0)
        {
            return;
        }
        inventoryMoveIndex -= 1;

    }

    public void HideInventory()
    {
        // Hide inventory UI  
        Debug.Log("Inventory hidden");
    }

    [SerializeField]
    List<GameObject> inventoryList = new List<GameObject>();
    Dictionary<InventoryType,GameObject> ItemDictionary = new Dictionary<InventoryType, GameObject>();

    private List<InventoryType> orderedKeys; // 정렬된 키 리스트
    private int inventoryMoveIndex = 0;

}
