using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
public enum EventType
{
    OpenPrisonDoor,
    ClosePrisonDoor,
    AllGeneratorSuccess, 
    UseEnergyDrink,
    UseInvisiblePotion,
    UseUpgradedLight,
    UsePrisonKey,
    UseHatch,
    UseMap,
    PlayerHpOne,
    PlayerHpZero,
    LightOn,
    LightOff,
    InEventPlayer,
    OutEventPlayer,
    LightRestored,
    SpawnMinigame,
    DestroyMiniGame,
    GeneratorSuccess,
}



public class EventManager
{
    private static readonly Dictionary<EventType, Action> _eventMap = new();

    public static Dictionary<EventType, Action> GeteventMap()
    {
        return _eventMap;
    }

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
    public static void UnRegisterEvent(EventType eventType)
    {
        if (_eventMap.ContainsKey(eventType))
            _eventMap[eventType] = null;
    }

    // �̺�Ʈ ����
    public static void TriggerEvent(EventType eventType)
    {
        _eventMap.TryGetValue(eventType, out var action);
        action?.Invoke();
    }
}
