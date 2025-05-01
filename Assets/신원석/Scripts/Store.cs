using Photon.Pun;
using UnityEngine;

public class Store : MonoBehaviour
{

    private void Start()
    {
        parentTransformObject = GameObject.Find("StoreWindowManager");
    }

    private void Update()
    {
        Debug.Log("isPlayerInRange: " + isPlayerInRange);

        if (isPlayerInRange && Input.GetKeyDown(KeyCode.E) && isCheck ==true)
        {
            isCheck = false;
            OpenStore();
        }
        else if (!isPlayerInRange && Input.GetKeyDown(KeyCode.E))
        {
            CloseStore();
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        PhotonView pv = collision.GetComponent<PhotonView>();
        if (pv != null && pv.IsMine && collision.CompareTag("Player"))
        {
            isPlayerInRange = true;
        }

    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        PhotonView pv = collision.GetComponent<PhotonView>();
        if (pv != null && pv.IsMine && collision.CompareTag("Player"))
        {
            isPlayerInRange = false;
            CloseStore();
            isCheck = true;
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

    private GameObject currentStoreWindow;

    private bool isPlayerInRange = false;

    private bool isCheck = true;

    [SerializeField]
    private GameObject storeWindow;
}