using System.Collections;
using UnityEngine;

public class MapTrigger : MonoBehaviour
{
    [Header("이동할 맵")]
    public GameObject moveMap;

    [Header("이동할 곳 이름")]
    public string Portal;

    private static bool isTeleport = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!(other.CompareTag("Player") || other.CompareTag("Ghost"))) return;
        if (isTeleport) return;

        MapManager mapManager = Object.FindFirstObjectByType<MapManager>();
        if (mapManager == null) return;

        isTeleport = true;

        GameObject newMap = mapManager.MoveMap(moveMap);
        Debug.Log(newMap);
        Transform returnTrigger = newMap.transform.Find(Portal);
        Debug.Log(returnTrigger);
        Debug.Log(other.transform.position);
        other.transform.position = returnTrigger.position;

        StartCoroutine(TeleportCooldown());
    }
    private IEnumerator TeleportCooldown()
    {
        yield return new WaitForSeconds(0.5f);
        isTeleport = false;
    }
}
