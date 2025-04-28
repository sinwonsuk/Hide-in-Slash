using System;
using UnityEngine;
using UnityEngine.UI;

public class Heart : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        hpOnedAction = ChangeHpOne;
        hpZeroAction = ChangeHpZero;

        EventManager.RegisterEvent(EventType.PlayerHpOne, hpOnedAction);
        EventManager.RegisterEvent(EventType.PlayerHpZero, hpZeroAction);
    }

    void ChangeHpOne()
    {
        images[1].sprite = Resources.Load<Sprite>("-hpIcon");
    }
    void ChangeHpZero()
    {
        images[0].sprite = Resources.Load<Sprite>("-hpIcon");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    [SerializeField]
    Image[] images;

    Action hpOnedAction;


    Action hpZeroAction;


}
