using UnityEngine;

public class titleButton : MonoBehaviour
{
    [Header("�α��� â")]
    [SerializeField] private GameObject login;

    public void OnClickStart()
    {
        Time.timeScale = 0;
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
