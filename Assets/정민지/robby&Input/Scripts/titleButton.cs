using UnityEngine;

public class titleButton : MonoBehaviour
{
    [Header("�α��� â")]
    [SerializeField] private GameObject login;

    [SerializeField] private GameObject Manager;

    void Start()
    {
        Instantiate(Manager);
    }

    public void OnClickStart()
    {
        Instantiate(login);


    }

    public void OnClickOption()
    {

    }

    public void OnClickExit()
    {
        Application.Quit();
    }
}
