using Photon.Pun;
using UnityEngine;

public class MiniGameZone : MonoBehaviour
{
    [SerializeField] private GameObject miniGamePrefab;
    private void OnEnable()
    {
        EventManager.RegisterEvent(EventType.SpawnMinigame, SpawnMinigame);
        EventManager.RegisterEvent(EventType.DestroyMiniGame, DestroyMiniGame);
    }

    private void OnDisable()
    {
        EventManager.UnRegisterEvent(EventType.SpawnMinigame, SpawnMinigame);
        EventManager.UnRegisterEvent(EventType.DestroyMiniGame, DestroyMiniGame);
    }

    private void SpawnMinigame()
    {

    }

    private void DestroyMiniGame()
    {

    }
}
