
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
    Map,
}

public class Inventory : MonoBehaviour
{

    private bool isInPrisonDoor = false; // player 감옥트리거 판단bool저장

    private void OnEnable()
    {
        EventManager.RegisterEvent(EventType.InPrisonDoor, CheckInPrisonDoor);
    }

    private void OnDisable()
    {
        EventManager.UnRegisterEvent(EventType.InPrisonDoor, CheckInPrisonDoor);
    }

    private void CheckInPrisonDoor(object data)
    {
        isInPrisonDoor = (bool)data;
    }


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
        if (Input.GetKeyDown(KeyCode.Alpha6))
        {
            if (ItemDictionary.Count < 6)
            {
                return;
            }
            inventoryMoveIndex = 5;
        }
    }


    public void AddItem(int _type,int price)
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
            TryUseSelectedItem();
        }      
    }


    private void TryUseSelectedItem()
    {
        InventoryType key = orderedKeys[inventoryMoveIndex];

        if (ItemDictionary.TryGetValue(key, out GameObject go))
        {
            if (key == InventoryType.PrisonKey && isInPrisonDoor == false)
                return;

            Item item = go.GetComponent<Item>();
            item.MusItemCount();

            UstItemPlayer(key);

            if (item.GetItemCount() == 0)
            {
                DeleteItem(go);
            }   
        }
    }

    void UstItemPlayer(InventoryType key)
    {
        switch (key)
        {
            case InventoryType.Invisibility:
                {
                    EventManager.TriggerEvent(EventType.UseInvisiblePotion);
                    break;
                }
            case InventoryType.Tunnel:
                {
                    ExitItemUI.SetActive(true);
                    EventManager.TriggerEvent(EventType.UseHatch);
                    break;
                }
            case InventoryType.Flashlight:
                {
                    EventManager.TriggerEvent(EventType.UseUpgradedLight);
                    break;
                }
            case InventoryType.EngeryDrink:
                {
                    EventManager.TriggerEvent(EventType.UseEnergyDrink);
                    break;
                }
            case InventoryType.PrisonKey:
                {

                    EventManager.TriggerEvent(EventType.UsePrisonKey);
                    break;
                }
            case InventoryType.Map:
                {
                    MapUI.SetActive(true);
                    EventManager.TriggerEvent(EventType.UseMap);
                    break;
                }
            default:
                break;
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

    private List<InventoryType> orderedKeys; // ���ĵ� Ű ����Ʈ

    [SerializeField]
    List<GameObject> itemNumbersRenders = new List<GameObject>();

    [SerializeField]
    GameObject ExitItemUI;
    [SerializeField]
    GameObject MapUI;

    private int inventoryMoveIndex = 0;

}
