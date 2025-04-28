using Photon.Pun;
using UnityEngine;

public class Store : MonoBehaviourPunCallbacks
{

    private void Start()
    {
        parentTransformObject = GameObject.Find("StoreWindowManager");

    }

    private void Update()
    {
        Debug.Log("isPlayerInRange: " + isPlayerInRange);

        if (isPlayerInRange && Input.GetKeyDown(KeyCode.E))
        {
            OpenStore();
        }
        else if (!isPlayerInRange && Input.GetKeyDown(KeyCode.E))
        {
            CloseStore();
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isPlayerInRange = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isPlayerInRange = false;
            CloseStore();
        }
    }

    private void OpenStore()
    {
        GameObject instantiate = Instantiate(storeWindow, parentTransformObject.transform);
        instantiate.GetComponent<StoreWindowMove>().isMove = true;

        currentStoreWindow = instantiate;
    }

    private void CloseStore()
    {
        if (currentStoreWindow != null)
        {
            currentStoreWindow.GetComponent<StoreWindowMove>().isMove = false;
        }      
    }
    [SerializeField]
    private GameObject parentTransformObject;

    [SerializeField]
    private GameObject currentStoreWindow;

    private bool isPlayerInRange = false;

    [SerializeField]
    private GameObject storeWindow;
}