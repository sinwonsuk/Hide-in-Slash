using UnityEngine;

public class loginButton : MonoBehaviour
{
    [SerializeField] private PlayerNameInput input;

    public void OnClick()
    {
        input.SavePlayerName();
    }
}
