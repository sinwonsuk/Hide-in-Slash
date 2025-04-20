using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public enum ColorState
{
    Red,
    Green,
    Blue,
    Pink,
    Black,

}


public class MemoryMiniGameColorChange : MonoBehaviour
{
    
    void Start()
    {
        memoryMiniGame = GetComponentInParent<MemoryMiniGame>();
        image = GetComponent<Image>();
        image.color = Color.white;
        StartCoroutine(ColorCorution());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator ColorCorution()
    {
        for (int i = 0; i < memoryMiniGame.memoryCount; i++)
        {
            int randomColor = Random.Range(0, 5);
            yield return StartCoroutine(ChangeColor((ColorState)randomColor));
            yield return null;
        }                    
    }

    IEnumerator ChangeColor(ColorState colorState)
    {      
        while (true)
        {
            time += Time.deltaTime;

            if (time > duration*2.0f)
            {
                memoryColorAction.Invoke(colorState);
                time = 0;
                yield break;
            }
            float t = Mathf.PingPong(Time.time, duration) / duration;
            Color currentColor = Color.Lerp(Color.white, colors[(int)colorState], t);
            image.color = currentColor;
            yield return null;
        }     
    }

    [SerializeField]
    List<Color> colors = new List<Color>();

    MemoryMiniGame memoryMiniGame;

    public event Action<ColorState> memoryColorAction;
    public float duration = 2f;
    Image image;
    float time;

}
