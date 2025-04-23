using UnityEngine;

public class titleButton : MonoBehaviour
{
    [Header("로그인 창")]
    [SerializeField] private GameObject login;

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
