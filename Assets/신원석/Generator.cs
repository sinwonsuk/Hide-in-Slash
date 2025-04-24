using UnityEngine;

public class Generator : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(isPlayerColilision ==true)
        {
            if (Input.GetKey(KeyCode.E))
            {
                canvas.gameObject.SetActive(true);
            }
            else
            {
                canvas.gameObject.SetActive(false);
            }
        }
     
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                isPlayerColilision = true;
            }
        }     
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        isPlayerColilision = false;

        if (collision.CompareTag("Player"))
        {
            canvas.gameObject.SetActive(false);
        }
    }

    bool isPlayerColilision;

    [SerializeField]
    Canvas canvas;
}
