using Photon.Pun;
using TMPro;
using UnityEngine;

public class roomNameText : MonoBehaviour
{
    [SerializeField] private TMP_Text rN;
    void Start()
    {
        rN.text = PhotonNetwork.CurrentRoom.Name;
    }

}
