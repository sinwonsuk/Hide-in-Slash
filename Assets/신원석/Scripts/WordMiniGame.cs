using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

enum adsasdsad
{ 
    Idle,
    Move,
    Attack,
}



public class WordMiniGame : MonoBehaviour
{
   

    void Start()
    {
        wordhandler = GetInputText;

        rectTransform = GetComponent<RectTransform>();

        StartCoroutine(StartWordMiniGame());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        Vector2 adsd =transform.position;
        Vector2 ssss = collision.transform.position;

        Debug.Log(gameObject.name);
        WordMiniGame ad = this;

        Destroy(collision.gameObject);
        wordList.Remove(collision.gameObject);                          
    }


    void OnCollisionEnter2D(UnityEngine.Collision2D collision)
    {
        Destroy(collision.gameObject);
        wordList.Remove(collision.gameObject);
    }

    public void GetInputText(string _text)
    {
        List<MiniGameWordDrop> sameStrings = new List<MiniGameWordDrop>();

        for (int i = 0; i < wordList.Count; i++)
        {
            MiniGameWordDrop word = wordList[i].GetComponent<MiniGameWordDrop>();

            if (word.textMeshProUGUI.text == _text)
            {
                sameStrings.Add(word);
            }
        }

        // 같은 이름중에 vector2 작은거 비교 
        if (sameStrings.Count > 1)
        {
            MiniGameWordDrop temp = sameStrings[0]; // 처음 거로 초기화

            for (int i = 1; i < sameStrings.Count; i++)
            {
                if (sameStrings[i].transform.position.y < temp.transform.position.y)
                {
                    temp = sameStrings[i];
                }
            }

            Destroy(temp.gameObject);
            wordList.Remove(temp.gameObject);
        }
        else if (sameStrings.Count == 1)
        {
            Destroy(sameStrings[0].gameObject);
            wordList.Remove(sameStrings[0].gameObject);
        }
    }


    IEnumerator StartWordMiniGame()
    {
        while (true)
        {
            float randomX = Random.Range(-340.0f, 340.0f);

            GameObject word = Instantiate(miniGameWordDrop, rectTransform);

            word.GetComponent<RectTransform>().anchoredPosition = new Vector2(randomX, 375.0f);

            wordList.Add(word);

            yield return new WaitForSeconds(1f);
        }
       
    }

    RectTransform rectTransform;    

    [SerializeField]
    GameObject miniGameWordDrop;

    List<GameObject> wordList = new List<GameObject>();

    public Action<string> wordhandler;

}
