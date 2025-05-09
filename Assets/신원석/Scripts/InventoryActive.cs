using UnityEngine;

public class InventoryActive : MonoBehaviour
{

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        EventManager.RegisterEvent(EventType.InevntoryOff, InventoryActiveOff);
    }

    private void OnDestroy()
    {
        EventManager.UnRegisterEvent(EventType.InevntoryOff, InventoryActiveOff);
    }

    public void InventoryActiveOff()
    {
        gameObject.SetActive(false);
    }


    // Update is called once per frame
    void Update()
    {

    }
}
