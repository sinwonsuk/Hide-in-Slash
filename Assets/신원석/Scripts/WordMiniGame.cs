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



public class WordMiniGame : MiniGame
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
        if(correctCheck == 5)
        {
            Instantiate(minigameSucess);
            DeleteAll();
        }

        if(Input.GetKeyDown(KeyCode.Escape))
        {
            DeleteAll();
        }

    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        Destroy(collision.gameObject);
        wordList.Remove(collision.gameObject);                          
    }


    void OnCollisionEnter2D(Collision2D collision)
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

            correctCheck++;
            Destroy(temp.gameObject);
            wordList.Remove(temp.gameObject);
        }
        else if (sameStrings.Count == 1)
        {
            correctCheck++;
            Destroy(sameStrings[0].gameObject);
            wordList.Remove(sameStrings[0].gameObject);
        }
    }


    IEnumerator StartWordMiniGame()
    {
        while (true)
        {
            int RandomObeect = Random.Range(0, 8);
            float randomX = Random.Range(-340.0f, 340.0f);
            // 8개중에 7개는 단어가 떨어지게 하고 나머지는 배터리 떨어지게 하기 위해서

            if (RandomObeect == 7)
            {
                GameObject betteryIns = Instantiate(bettery, rectTransform);
                betteryIns.GetComponent<RectTransform>().anchoredPosition = new Vector2(randomX, 375.0f);
                batteryList.Add(betteryIns);
            }
            else
            {              
                GameObject word = Instantiate(miniGameWordDrop, rectTransform);
                word.GetComponent<RectTransform>().anchoredPosition = new Vector2(randomX, 375.0f);
                wordList.Add(word);
            }
            yield return new WaitForSeconds(1f);
        }      
    }

    void DeleteAll()
    {
        trigerAction.Invoke();
        Destroy(transform.parent.gameObject);

        for (int i = 0; i < wordList.Count; i++)
        {
            Destroy(wordList[i].gameObject);
        }
        for (int i = 0; i < batteryList.Count; i++)
        {
            Destroy(batteryList[i].gameObject);
        }
    }

    RectTransform rectTransform;    

    [SerializeField]
    GameObject miniGameWordDrop;
    [SerializeField]
    GameObject bettery;
    [SerializeField]
    GameObject minigameSucess;

    List<GameObject> wordList = new List<GameObject>();
    List<GameObject> batteryList = new List<GameObject>();

    public Action<string> wordhandler;

    [SerializeField]
    int correctCount = 10;

    int correctCheck = 0;


}
