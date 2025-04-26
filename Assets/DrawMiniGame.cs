using UnityEngine;
using UnityEngine.Events;

public class DrawMiniGame : MiniGame
{
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
        Delete();
       Instantiate(sucessObject);
    }
    private void CreateFailObject()
    {
        Delete();
        Instantiate(failObject);
    }


    void Delete()
    {
        trigerAction.Invoke();
        Destroy(gameObject);
    }

    public UnityAction sucessObjectAction;
    public UnityAction failObjectAction;

    public UnityAction Action;

    [SerializeField]
    GameObject sucessObject;

    [SerializeField]
    GameObject failObject;
}
