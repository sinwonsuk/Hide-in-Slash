using System.Collections.Generic;
using UnityEngine;

public enum MinigameState
{
    DrawMiniGame,
    LineMiniGame,
    CatchMiniGame,
    MemoryMiniGame,
    WordMiniGame,
}

public class MiniGameManager : MonoBehaviour
{


    private void Awake()
    {
        minigameDictory = new Dictionary<MinigameState, GameObject>();

        foreach (var entry in minigameEntries)
        {
            if (!minigameDictory.ContainsKey(entry.state))
            {
                minigameDictory.Add(entry.state, entry.prefab);
            }          
        }

        LaunchRandomMinigame();
    }

    private void Start()
    {
        //LaunchRandomMinigame();
    }

    public void LaunchRandomMinigame()
    {
        List<MinigameState> keys = new List<MinigameState>(minigameDictory.Keys);

        MinigameState randomKey = keys[Random.Range(0, keys.Count)];

        choiceMiniGame = Instantiate(minigameDictory[randomKey],transform);

        Vector2 camera = Camera.main.transform.position;

        choiceMiniGame.transform.position = new Vector2(camera.x,camera.y);
    }

    [System.Serializable]
    public struct MinigameEntry
    {
        public MinigameState state;
        public GameObject prefab;
    }

    [SerializeField]
    private List<MinigameEntry> minigameEntries;

    private Dictionary<MinigameState, GameObject> minigameDictory;

    public GameObject choiceMiniGame { get; set; }
}