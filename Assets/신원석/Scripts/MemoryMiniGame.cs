using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;

public class MemoryMiniGame : MonoBehaviour
{

   
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        memoryMiniGameColorChange = GetComponentInChildren<MemoryMiniGameColorChange>();
        memoryMiniGameColorChange.memoryColorAction += MemoryMiniGame_OnColorChange;
    }

    private void MemoryMiniGame_OnColorChange(ColorState colorState)
    {
        colors.Add(colorState);
    }  

    public bool ActiveButton()
    {
        return memoryCount == colors.Count;
    }

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
                    Debug.Log("미니게임 실패");
                    return;
                }
            }
            Debug.Log("미니게임 성공");

        }
    }


    void Update()
    {
       
    }


    int colorindex = 0;
    public int memoryCount { get; set; } = 5;

    List<bool> colorcheck = new List<bool>();

    List<ColorState> colors = new List<ColorState>();
    MemoryMiniGameColorChange memoryMiniGameColorChange;
}
