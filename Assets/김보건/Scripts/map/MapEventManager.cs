using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEditor.Searcher.Searcher.AnalyticsEvent;

public enum MapEventType
{
    OpenPrisonDoor,
    ClosePrisonDoor,
    GeneratorStart,
    GeneratorSuccess,
    miniGameStart,
    miniGameSuccess,
}

public class MapEventManager
{
    private static Dictionary<MapEventType, Action> eventMap = new();

    // ��ȣ
    public static void RegisterEvent(MapEventType eventType, Action eventFunc)
    {
        if (!eventMap.ContainsKey(eventType))
            eventMap[eventType] = eventFunc;
        else
            eventMap[eventType] += eventFunc;
    }

    // ��ȣ�ޱ�
    public static void UnRegisterEvent(MapEventType eventType, Action eventFunc)
    {
        if (eventMap.ContainsKey(eventType))
            eventMap[eventType] -= eventFunc;
    }

    // �̺�Ʈ �߻�
    public static void TriggerEvent(MapEventType eventType)
    {
        if (eventMap.ContainsKey(eventType))
        {
            Action action = eventMap[eventType];

            if (action != null)
            {
                action.Invoke(); 
            }
        }
    }
}
