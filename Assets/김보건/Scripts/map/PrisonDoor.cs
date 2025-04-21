using System.Collections;
using UnityEngine;

public class PrisonDoor : MonoBehaviour
{
    private Animator anim;
    [SerializeField] private GameObject prisonDoor;

    private void Awake()
    {
        anim = GetComponent<Animator>();
    }

    private void OnEnable()
    {
        MapEventManager.RegisterEvent(MapEventType.OpenPrisonDoor, OpenDoor);
        MapEventManager.RegisterEvent(MapEventType.ClosePrisonDoor, CloseDoor);
    }

    private void OnDisable()
    {
        MapEventManager.UnRegisterEvent(MapEventType.OpenPrisonDoor, OpenDoor);
        MapEventManager.UnRegisterEvent(MapEventType.ClosePrisonDoor, CloseDoor);
    }

    private void OpenDoor()
    {
        Debug.Log("���� �� ����");
        //anim.SetTrigger("Open");  �ִϸ��̼�
        prisonDoor.SetActive(false);
        StartCoroutine(CloseDoorDelay(5f));
    }

    private void CloseDoor()
    {
        Debug.Log("���� �� ����");
        //anim.SetTrigger("Close");   
        prisonDoor.SetActive(true);
    }

    private IEnumerator CloseDoorDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        CloseDoor();
    }
}
