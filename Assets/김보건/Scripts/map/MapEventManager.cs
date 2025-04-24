using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
public enum MapEventType
{
    OpenPrisonDoor,
    ClosePrisonDoor,
    miniGameSuccess,
    GeneratorSuccess,
    AllGeneratorSuccess,
    PosirionFunction,
    UseEnergyDrink,
    UseInvisiblePotion,
    UseUpgradedLight,
    UsePrisonKey,
    UseHatch
}



public class MapEventManager
{
    private static readonly Dictionary<MapEventType, Action> _eventMap = new();

    // �̺�Ʈ ���
    public static void RegisterEvent(MapEventType eventType, Action eventFunc)
    {
        if (!_eventMap.ContainsKey(eventType))
            _eventMap[eventType] = eventFunc;
        else
            _eventMap[eventType] += eventFunc;
    }

    // �̺�Ʈ ��� ����
    public static void UnRegisterEvent(MapEventType eventType, Action eventFunc)
    {
        if (_eventMap.ContainsKey(eventType))
            _eventMap[eventType] -= eventFunc;
    }

    // �̺�Ʈ ����
    public static void TriggerEvent(MapEventType eventType)
    {
        _eventMap.TryGetValue(eventType, out var action);
        action?.Invoke();
    }
}
