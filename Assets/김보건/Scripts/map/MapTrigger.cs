using Photon.Pun;
using System;
using System.Collections;
using Unity.Cinemachine;
using UnityEngine;

public class MapTrigger : MonoBehaviour
{
    [Header("이동할 맵")]
    public GameObject moveMap;

    [Header("이동할 곳 이름")]
    public string Portal;

    private static bool isTeleport = false;
    private Canvas canvas;
    private void Start()
    {
        canvas = GameObject.FindGameObjectWithTag("Dark").GetComponent<Canvas>();
        canvas.enabled = false;
    }

 

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!(other.CompareTag("Player") || other.CompareTag("Ghost"))) return;
        if (isTeleport) return;

        AssignManager mapManager = AssignManager.instance;
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
        canvas.enabled = true;
        yield return new WaitForSeconds(0.05f);
        confiner.InvalidateBoundingShapeCache();
        yield return new WaitForSeconds(0.25f);
        canvas.enabled = false;

    }
}
