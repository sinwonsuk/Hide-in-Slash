using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class MemoryMiniGame : MiniGame
{

   
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Vector2 camera = Camera.main.transform.position;
        transform.position = camera;


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
                    Delete();
                    Instantiate(failObject);
                    Debug.Log("미니게임 실패");
                    return;
                }
            }

            Debug.Log("미니게임 성공");
            Delete();
            Money.instance.addMoney(money);
            Instantiate(sucessObject);
           

        }
    }

    void Delete()
    {
        trigerAction.Invoke();
        Destroy(gameObject);
    }


    void Update()
    {
       
    }

    public UnityAction<ColorState> colorChangeAction;
    public UnityAction<int> checkColorAction;
    public Func<bool> activeButtonAction;

    int colorindex = 0;
    public int memoryCount { get; set; } = 3;

    List<bool> colorcheck = new List<bool>();

    List<ColorState> colors = new List<ColorState>();

    [SerializeField]
    GameObject sucessObject;

    [SerializeField]
    GameObject failObject;
    [SerializeField]
    int money = 0;
}
