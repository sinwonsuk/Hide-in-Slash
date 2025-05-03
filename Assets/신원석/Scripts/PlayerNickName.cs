using Photon.Pun;
using TMPro; // 또는 UnityEngine.UI 사용

public class PlayerNickName : MonoBehaviourPun
{
    public TextMeshProUGUI nicknameText;

    void Start()
    {
        if (photonView.IsMine)
        {
            
        }
        else
        {
            nicknameText.text = photonView.Owner.NickName;
        }
    }

    void Update()
    {
        
    }
}