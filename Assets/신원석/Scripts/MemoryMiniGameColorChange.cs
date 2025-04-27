using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public enum ColorState
{
    Red,
    Green,
    Blue,
    Pink,
    Yellow,

}


public class MemoryMiniGameColorChange : MonoBehaviour
{
    
    void Start()
    {
        memoryMiniGame = GetComponentInParent<MemoryMiniGame>();
        spriteRender = GetComponent<SpriteRenderer>();
        spriteRender.color = Color.white;
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

        light2D.color = colors[(int)colorState];
        float t = 0;
        while (true)
        {
            time += Time.deltaTime;



            if (time > duration* 2.0f && t < 0.05f)
            {
                light2D.intensity = 0;
                memoryMiniGame.colorChangeAction.Invoke(colorState);
                time = 0;
                yield break;
            }

           
            t = Mathf.PingPong(Time.time, duration) / duration;

            light2D.intensity =Mathf.Lerp(0, 18, t);


            //Color currentColor = Color.Lerp(Color.white, colors[(int)colorState], t);        
            yield return null;
        }     
    }

    [SerializeField]
    List<Color> colors = new List<Color>();

    MemoryMiniGame memoryMiniGame;

    public float duration = 2f;

    [SerializeField]
    Light2D light2D;

    SpriteRenderer spriteRender;
    float time;

}
