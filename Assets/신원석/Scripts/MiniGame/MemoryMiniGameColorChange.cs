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
        light2D.color = Color.white;
        light2D.intensity = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if(coroutine == null)
        {
            coroutine = StartCoroutine(ColorCorution());
        }
       
    }

    IEnumerator ColorCorution()
    {
        yield return new WaitForSeconds(1.0f);

        for (int i = 0; i < memoryMiniGame.memoryCount; i++)
        {
            Debug.Log("획수" +memoryMiniGame);
            int randomColor = Random.Range(0, 5);
            yield return StartCoroutine(ChangeColor((ColorState)randomColor));
            yield return null;
        }                    
    }

    IEnumerator ChangeColor(ColorState colorState)
    {
        light2D.intensity = 0;
        yield return new WaitForSeconds(0.1f);  // 깜빡임 전 준비시간

        light2D.color = colors[(int)colorState];

        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;  // 0 → 1
            light2D.intensity = Mathf.Lerp(0, 18, t);  // 밝아짐
            yield return null;
        }

        elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;  // 0 → 1
            light2D.intensity = Mathf.Lerp(18, 0, t);  // 다시 어두워짐
            yield return null;
        }

        light2D.intensity = 0;
        memoryMiniGame.colorChangeAction.Invoke(colorState);
    }

    [SerializeField]
    List<Color> colors = new List<Color>();

    MemoryMiniGame memoryMiniGame;

    public float duration = 2f;

    [SerializeField]
    Light2D light2D;

    SpriteRenderer spriteRender;
    float time;

    Coroutine coroutine;
}
