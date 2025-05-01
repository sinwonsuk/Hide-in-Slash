using NUnit.Framework.Constraints;
using Photon.Pun;
using System.Collections;
using UnityEngine;

public class PrisonDoor : MonoBehaviourPunCallbacks
{
    private Animator anim;
    [SerializeField] private GameObject prisonDoor;

    private void Awake()
    {
        anim = GetComponent<Animator>();
    }

    private new void OnEnable()
    {
        EventManager.RegisterEvent(EventType.OpenPrisonDoor, OpenDoor);
        EventManager.RegisterEvent(EventType.ClosePrisonDoor, CloseDoor);
    }

    private new void OnDisable()
    {
        EventManager.UnRegisterEvent(EventType.OpenPrisonDoor, OpenDoor);
        EventManager.UnRegisterEvent(EventType.ClosePrisonDoor, CloseDoor);
    }

    private void OpenDoor()
    {
        photonView.RPC("OpenDoorRPC", RpcTarget.All);
    }

    private void CloseDoor()
    {
        photonView.RPC("CloseDoorRPC", RpcTarget.All);
    }

    private IEnumerator CloseDoorDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        photonView.RPC("CloseDoorRPC", RpcTarget.All);
    }

    [PunRPC]
    private void OpenDoorRPC()
    {
        Debug.Log("감옥 문 열림");
        prisonDoor.SetActive(false);
        StartCoroutine(CloseDoorDelay(3f));
    }

    [PunRPC]
    private void CloseDoorRPC()
    {
        Debug.Log("감옥 문 닫힘");
        prisonDoor.SetActive(true);
    }
}
