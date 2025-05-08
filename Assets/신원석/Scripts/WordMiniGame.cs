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

        EventManager.TriggerEvent(EventType.ChattingActiveOff);

    }

    // Update is called once per frame
    void Update()
    {
        if(correctCheck == 3)
        {
            Money.instance.addMoney(money);
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
            MiniGameWordDrop word = wordList[i].GetComponentInChildren<MiniGameWordDrop>();

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
            Destroy(temp.transform.parent.parent.gameObject);
            wordList.Remove(temp.transform.parent.parent.gameObject);
        }
        else if (sameStrings.Count == 1)
        {
            correctCheck++;
            Destroy(sameStrings[0].transform.parent.parent.gameObject);
            wordList.Remove(sameStrings[0].transform.parent.parent.gameObject);
        }
    }


    IEnumerator StartWordMiniGame()
    {
        while (true)
        {
            int RandomObeect = Random.Range(0, 8);
            float randomX = Random.Range(-340.0f, 340.0f);

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
        EventManager.TriggerEvent(EventType.ChattingActiveOn);

        trigerAction.Invoke();
        Destroy(transform.parent.gameObject);

        for (int i = 0; i < wordList.Count; i++)
        {
            Destroy(wordList[i].gameObject);
        }
    }

    RectTransform rectTransform;    

    [SerializeField]
    GameObject miniGameWordDrop;
    [SerializeField]
    GameObject minigameSucess;

    List<GameObject> wordList = new List<GameObject>();

    public Action<string> wordhandler;

    int correctCheck = 0;

    [SerializeField]
    int money = 0;

}
