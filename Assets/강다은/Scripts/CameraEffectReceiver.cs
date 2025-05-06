using System.Collections;
using UnityEngine;
using Photon.Pun;
using Unity.Cinemachine;

public class CameraEffectReceiver : MonoBehaviourPun
{
    private CinemachineCamera cam;
    private Camera mainCam;
    private void Start()
    {
        if (!photonView.IsMine) return;

        cam = FindFirstObjectByType<CinemachineCamera>();
        mainCam = Camera.main;
    }

    [PunRPC]
    public void RotateSurvivorScreen(float angle, float duration)
    {
        if (!photonView.IsMine || cam == null) return;

        Debug.Log($"플레이어 화면 회전 : {angle}도, 지속시간 {duration}s");
        StartCoroutine(RotateVirtualCameraCoroutine(cam.transform, mainCam.transform, angle, duration));
    }

    private IEnumerator RotateVirtualCameraCoroutine(Transform camTransform, Transform mainTransform, float angle, float duration)
    {
        Quaternion originalRot = camTransform.rotation;
        Quaternion originalMainRot = mainTransform.rotation;

        Camera camComponent = mainTransform.GetComponent<Camera>();
        float originalSize = camComponent.orthographicSize;
        camComponent.orthographicSize -= 1.5f;

        Quaternion targetRotation = Quaternion.Euler(0, 0, angle);
        camTransform.rotation = targetRotation;
        mainTransform.rotation = targetRotation;

        yield return new WaitForSeconds(duration);

        camTransform.rotation = originalRot;
        mainTransform.rotation = originalMainRot;
        camComponent.orthographicSize = originalSize;
    }
}
