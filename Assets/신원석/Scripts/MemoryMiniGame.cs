using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class MemoryMiniGame : MonoBehaviour
{

   
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        colorChangeAction = MemoryMiniGame_OnColorChange;
        checkColorAction = CheckColor;
        activeButtonAction = ActiveButton;
    }

    private void MemoryMiniGame_OnColorChange(ColorState colorState)
    {
        colors.Add(colorState);
    }

    bool ActiveButton() => memoryCount == colors.Count;

    public void CheckColor(int color)
    {     
        if (colors[colorindex] == (ColorState)color)
        {
            colorcheck.Add(true);
            Debug.Log("Correct Color");
        }
        else
        {
            colorcheck.Add(false);
            Debug.Log("Wrong Color");
        }

        colorindex++;

        if (colorindex == memoryCount)
        {
            colorindex = 0;
            colors.Clear();

            for (int i = 0; i < memoryCount; i++)
            {
                if (colorcheck[i] == false)
                {
                    Debug.Log("�̴ϰ��� ����");
                    return;
                }
            }
            Debug.Log("�̴ϰ��� ����");

        }
    }


    void Update()
    {
       
    }

    public UnityAction<ColorState> colorChangeAction;
    public UnityAction<int> checkColorAction;
    public Func<bool> activeButtonAction;

    int colorindex = 0;
    public int memoryCount { get; set; } = 5;

    List<bool> colorcheck = new List<bool>();

    List<ColorState> colors = new List<ColorState>();
}
