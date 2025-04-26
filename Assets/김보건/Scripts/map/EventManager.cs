using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
public enum EventType
{
    OpenPrisonDoor,
    ClosePrisonDoor,
    miniGameSuccess,
    GeneratorSuccess,
    AllGeneratorSuccess, 
    UseEnergyDrink,
    UseInvisiblePotion,
    UseUpgradedLight,
    UsePrisonKey,
    UseHatch,
    LightOn,
    LightOff,
    UseMap,
    InEventPlayer,
    OutEventPlayer
}



public class EventManager
{
    private static readonly Dictionary<EventType, Action> _eventMap = new();

    // �̺�Ʈ ���
    public static void RegisterEvent(EventType eventType, Action eventFunc)
    {
        if (!_eventMap.ContainsKey(eventType))
            _eventMap[eventType] = eventFunc;
        else
            _eventMap[eventType] += eventFunc;
    }

    // �̺�Ʈ ��� ����
    public static void UnRegisterEvent(EventType eventType, Action eventFunc)
    {
        if (_eventMap.ContainsKey(eventType))
            _eventMap[eventType] -= eventFunc;
    }

    // �̺�Ʈ ����
    public static void TriggerEvent(EventType eventType)
    {
        _eventMap.TryGetValue(eventType, out var action);
        action?.Invoke();
    }
}
