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
        Debug.Log("°¨¿Á ¹® ¿­¸²");
        //anim.SetTrigger("Open");  ¾Ö´Ï¸ÞÀÌ¼Ç
        prisonDoor.SetActive(false);
        StartCoroutine(CloseDoorDelay(5f));
    }

    private void CloseDoor()
    {
        Debug.Log("°¨¿Á ¹® ´ÝÈû");
        //anim.SetTrigger("Close");   
        prisonDoor.SetActive(true);
    }

    private IEnumerator CloseDoorDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        CloseDoor();
    }
}
