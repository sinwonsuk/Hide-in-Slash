using NUnit.Framework;
using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

public class LineMiniGame : MiniGame
{

    private void Awake()
    {
        Vector2 camera = Camera.main.transform.position;

        transform.position = camera;

        action = CheckLineCount;

        LineMiniGameRightCollisions = GetComponentsInChildren<LineMiniGameRightCollision>();

        foreach (var LineMiniGameRightCollision in LineMiniGameRightCollisions)
        {
            Transform transform = LineMiniGameRightCollision.transform;

            childTransforms.Add(transform.position);
        }

        for (int i = 0; i < childTransforms.Count; i++)
        {
            int random = Random.Range(0, 4);
        }

        CreateUnDuplicateRandom(0, 4);

        for (int i = 0; i < LineMiniGameRightCollisions.Length; i++)
        {
            Transform target = LineMiniGameRightCollisions[i].transform;

            Vector2 anchoredPosition = childTransforms[Indexs[i]];

            target.transform.position = anchoredPosition;
        }
    }

    void CheckLineCount(int num)
    {
        for (int i = 0; i < lineChecks.Count; i++)
        {
            if (lineChecks[i]==num)
            {
                return;
            }
        }

        lineChecks.Add(num);
    
        if(lineChecks.Count == 4)
        {
            trigerAction.Invoke();
            minigameSucess =Instantiate(minigameSucess);
            Destroy(gameObject);
        }

    }

    void CreateUnDuplicateRandom(int min, int max)
    {
        int currentNumber = Random.Range(min, max);

        for (int i = 0; i < max;)
        {
            if (Indexs.Contains(currentNumber))
            {
                currentNumber = Random.Range(min, max);
            }
            else
            {
                Indexs.Add(currentNumber);
                i++;
            }
        }
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    LineMiniGameRightCollision[] LineMiniGameRightCollisions;
    List<Vector2> childTransforms = new List<Vector2>();
    List<int> Indexs = new List<int>();
    List<int> lineChecks = new List<int>();

    public Action<int> action;

    [SerializeField]
    GameObject minigameSucess;

}
 