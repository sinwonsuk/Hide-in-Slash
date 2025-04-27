using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class GeneratorManager : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        EventManager.RegisterEvent(EventType.GeneratorSuccess, addCount);
    }

    // Update is called once per frame
    void Update()
    {
        if(generatorCount == 5)
        {
            EventManager.TriggerEvent(EventType.AllGeneratorSuccess);
        }
    }

    public void addCount()
    {
        generatorCount++;
    }



    int generatorCount = 0;


}
