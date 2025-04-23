using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEditor.Searcher.Searcher.AnalyticsEvent;

public enum MapEventType
{
    OpenPrisonDoor,
    ClosePrisonDoor,
    miniGameSuccess,
    GeneratorSuccess,
    AllGeneratorSuccess,
}



public class MapEventManager
{
    private static readonly Dictionary<MapEventType, Action> _eventMap = new();

    // 이벤트 등록
    public static void RegisterEvent(MapEventType eventType, Action eventFunc)
    {
        if (!_eventMap.ContainsKey(eventType))
            _eventMap[eventType] = eventFunc;
        else
            _eventMap[eventType] += eventFunc;
    }

    // 이벤트 등록 해제
    public static void UnRegisterEvent(MapEventType eventType, Action eventFunc)
    {
        if (_eventMap.ContainsKey(eventType))
            _eventMap[eventType] -= eventFunc;
    }

    // 이벤트 실행
    public static void TriggerEvent(MapEventType eventType)
    {
        _eventMap.TryGetValue(eventType, out var action);
        action?.Invoke();
    }
}
