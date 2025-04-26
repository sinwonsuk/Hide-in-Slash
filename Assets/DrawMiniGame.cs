using UnityEngine;
using UnityEngine.Events;

public class DrawMiniGame : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Vector2 camera = Camera.main.transform.position;

        transform.position = camera;

        sucessObjectAction = CreateSucessObject;
        failObjectAction = CreateFailObject;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void CreateSucessObject()
    {
       Instantiate(sucessObject);
    }
    private void CreateFailObject()
    {
        Instantiate(failObject);
    }
    public UnityAction sucessObjectAction;
    public UnityAction failObjectAction;

    [SerializeField]
    GameObject sucessObject;

    [SerializeField]
    GameObject failObject;
}
