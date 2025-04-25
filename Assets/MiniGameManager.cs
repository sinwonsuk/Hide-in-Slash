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
    [System.Serializable]
    public struct MinigameEntry
    {
        public MinigameState state;
        public GameObject prefab;
    }

    [SerializeField]
    private List<MinigameEntry> minigameEntries;

    private Dictionary<MinigameState, GameObject> minigameDictory;

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
    }

    private void Start()
    {
        LaunchRandomMinigame();
    }

    private void LaunchRandomMinigame()
    {
        List<MinigameState> keys = new List<MinigameState>(minigameDictory.Keys);

        MinigameState randomKey = keys[Random.Range(0, keys.Count)];

        Instantiate(minigameDictory[randomKey]);
    }
}