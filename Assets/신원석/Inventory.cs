
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

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            if (0 >= ItemDictionary.Count - 1)
            {
                return;
            }

            inventoryMoveIndex = 0;
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            if (ItemDictionary.Count < 2)
            {
                return;
            }

            inventoryMoveIndex = 1;
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            if (ItemDictionary.Count < 3)
            {
                return;
            }
            inventoryMoveIndex = 2;
        }
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            if (ItemDictionary.Count < 4)
            {
                return;
            }
            inventoryMoveIndex = 3;
        }
        if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            if (ItemDictionary.Count < 5)
            {
                return;
            }
            inventoryMoveIndex = 4;
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
            item.GetComponentInChildren<ItemNumber>().ChangeImage(orderedKeys.Count);
        }

    }

    public void UseItem()
    {
        if(Input.GetKeyDown(KeyCode.Space) && ItemDictionary.Count > 0)
        {
            EventManager.TriggerEvent(EventType.UseMap);
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
        
        Destroy(gameObject);
        ItemDictionary.Remove(orderedKeys[inventoryMoveIndex]);
        orderedKeys.Remove(orderedKeys[inventoryMoveIndex]);


        for (int i = inventoryMoveIndex; i < orderedKeys.Count; i++)
        {
            ItemDictionary[orderedKeys[i]].GetComponentInChildren<ItemNumber>().ChangeImage(i+1);
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

    [SerializeField]
    List<GameObject> itemNumbersRenders = new List<GameObject>();

    private int inventoryMoveIndex = 0;

}
