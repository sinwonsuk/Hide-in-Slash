using UnityEngine;

public class ChattingActive : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        EventManager.RegisterEvent(EventType.ChattingOff, ChattingOff);
    }

    private void OnDestroy()
    {
        EventManager.UnRegisterEvent(EventType.ChattingOff);
    }

    public void ChattingOff()
    {
        gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
