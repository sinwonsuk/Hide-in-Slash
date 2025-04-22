using UnityEngine;

public class StartButton : MonoBehaviour
{
    [Header("로그인 창")]
    [SerializeField] private GameObject login;

    public void OnClick()
    {
        Time.timeScale = 0;
        Instantiate(login);
    }
}
