using UnityEngine;

public class StartButton : MonoBehaviour
{
    [Header("�α��� â")]
    [SerializeField] private GameObject login;

    public void OnClick()
    {
        Time.timeScale = 0;
        Instantiate(login);
    }
}
