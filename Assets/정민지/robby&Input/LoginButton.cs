using UnityEngine;

public class LoginButton : MonoBehaviour
{
    [SerializeField] private PlayerNameInput input;

    public void OnClick()
    {
        input.SavePlayerName();
    }
}
