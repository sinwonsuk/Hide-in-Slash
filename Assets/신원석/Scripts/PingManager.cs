using UnityEngine;

public class PingManager : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKey(KeyCode.LeftControl))
        {
            pingObject.SetActive(true);
        }
        else 
        {
            pingObject.SetActive(false);
        }
        
    }

    bool isCheck = false;

    [SerializeField]
    GameObject pingObject;
}
