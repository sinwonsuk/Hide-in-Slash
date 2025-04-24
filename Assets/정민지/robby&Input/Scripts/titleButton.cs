using UnityEngine;

public class titleButton : MonoBehaviour
{
    [Header("로그인 창")]
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
