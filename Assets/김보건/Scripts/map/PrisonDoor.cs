using NUnit.Framework.Constraints;
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
        EventManager.RegisterEvent(EventType.OpenPrisonDoor, OpenDoor);
        EventManager.RegisterEvent(EventType.ClosePrisonDoor, CloseDoor);
    }

    private void OnDisable()
    {
        EventManager.UnRegisterEvent(EventType.OpenPrisonDoor, OpenDoor);
        EventManager.UnRegisterEvent(EventType.ClosePrisonDoor, CloseDoor);
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
