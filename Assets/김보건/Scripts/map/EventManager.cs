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
    HasPrisonKey,
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
    InPrisonDoor,
    ChattingActiveOff,
    ChattingActiveOn,
    AddItem,
    EntireLightOn,
    EntireLightOff,
    InevntoryOff,
    ChattingOff,   
    PingOff,
}



public class EventManager
{
    private static readonly Dictionary<EventType, Action> _eventMap = new();
    private static readonly Dictionary<EventType, Action<object>> _eventMapWithEventData = new();

    public static Dictionary<EventType, Action> GeteventMap()
    {
        return _eventMap;
    }

    // 이벤트구독
    public static void RegisterEvent(EventType eventType, Action eventFunc)
    {
        if (!_eventMap.ContainsKey(eventType))
            _eventMap[eventType] = eventFunc;
        else
            _eventMap[eventType] += eventFunc;
    }
    public static void RegisterEvent(EventType eventType, Action<object> eventFunc)
    {
        if (!_eventMapWithEventData.ContainsKey(eventType))
            _eventMapWithEventData[eventType] = eventFunc;
        else
            _eventMapWithEventData[eventType] += eventFunc;
    }

    // 이벤트해제
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
    public static void UnRegisterEvent(EventType eventType, Action<object> eventFunc)
    {
        if (_eventMapWithEventData.ContainsKey(eventType))
            _eventMapWithEventData[eventType] -= eventFunc;
    }

    // 이벤트발생
    public static void TriggerEvent(EventType eventType)
    {
        _eventMap.TryGetValue(eventType, out var action);
        action?.Invoke();
    }

    public static void TriggerEvent(EventType eventType, object eventData)
    {
        if (_eventMapWithEventData.TryGetValue(eventType, out var action))
            action?.Invoke(eventData);
    }
}
