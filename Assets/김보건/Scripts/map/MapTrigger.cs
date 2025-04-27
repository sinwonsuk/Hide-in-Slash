using Photon.Pun;
using System.Collections;
using Unity.Cinemachine;
using UnityEngine;

public class MapTrigger : MonoBehaviour
{
    [Header("�̵��� ��")]
    public GameObject moveMap;

    [Header("�̵��� �� �̸�")]
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
        Transform returnTrigger = newMap.transform.Find(Portal);
        other.transform.position = returnTrigger.position;

        PhotonView pv = other.GetComponent<PhotonView>();
        CinemachineCamera cam = FindFirstObjectByType<CinemachineCamera>();
        CinemachineConfiner2D confiner = cam.GetComponent<CinemachineConfiner2D>();
        if (pv != null && pv.IsMine)
        {
            confiner.BoundingShape2D = newMap.GetComponent<Collider2D>();
            StartCoroutine(UpdateCameraConfinerDelayed());
        }


        StartCoroutine(TeleportCooldown());
    }
    private IEnumerator TeleportCooldown()
    {
        yield return new WaitForSeconds(0.5f);
        isTeleport = false;
    }

    private IEnumerator UpdateCameraConfinerDelayed()
    {
        CinemachineCamera cam = FindFirstObjectByType<CinemachineCamera>();
        CinemachineConfiner2D confiner = cam.GetComponent<CinemachineConfiner2D>();
        // �� ������ ���. ��Ȳ�� ���� WaitForEndOfFrame() ��� WaitForFixedUpdate()�� ����غ�����.
        yield return new WaitForSeconds(0.2f);
        confiner.InvalidateBoundingShapeCache();
    }
}
