using UnityEngine;

public class Store : MonoBehaviour
{

    private void Start()
    {
        storeWindow = GameObject.Find("StoreWindow");
        storeWindow.SetActive(false);
    }

    private void Update()
    {
        if (isPlayerInRange && Input.GetKeyDown(KeyCode.E))
        {
            OpenStore();
        }
        else if (isPlayerInRange ==false && Input.GetKeyDown(KeyCode.E))
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
        if (storeWindow != null)
        {
            storeWindow.SetActive(true);
        }
    }

    private void CloseStore()
    {
        if (storeWindow != null)
        {
            storeWindow.SetActive(false);
        }
    }

    [SerializeField] private GameObject storeWindow;
    private bool isPlayerInRange = false;
}