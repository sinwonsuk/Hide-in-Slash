using System.Collections;
using UnityEngine;
using Photon.Pun;
using Unity.Cinemachine;

public class CameraEffectReceiver : MonoBehaviourPun
{
    private CinemachineCamera cam;
    private void Start()
    {
        if (!photonView.IsMine) return;

        cam = FindFirstObjectByType<CinemachineCamera>();
    }

    [PunRPC]
    public void RotateSurvivorScreen(float angle, float duration)
    {
        if (!photonView.IsMine || cam == null) return;

        Debug.Log($"플레이어 화면 회전 : {angle}도, 지속시간 {duration}s");
        StartCoroutine(RotateVirtualCameraCoroutine(cam.transform, angle, duration));
    }

    private IEnumerator RotateVirtualCameraCoroutine(Transform camTransform, float angle, float duration)
    {
        Quaternion originalRot = camTransform.rotation;
        camTransform.rotation = Quaternion.Euler(0, 0, angle);

        yield return new WaitForSeconds(duration);

        camTransform.rotation = originalRot;
    }
}
