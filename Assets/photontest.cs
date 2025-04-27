using Photon.Pun;
using UnityEngine;

public class photontest : MonoBehaviour
{
    public ProfileSlotManager profileSlotManager;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
      //  PhotonNetwork.Instantiate("generator1", new Vector3(-3.0f,0.0f,0.0f), Quaternion.identity);
    }

    // Update is called once per frame
    void Update()
    {



        if(Input.GetKeyDown(KeyCode.O))
        {
            profileSlotManager.photonView.RPC("SyncProfileState", RpcTarget.All, PhotonNetwork.LocalPlayer, "Dead");
        }       
    }
}
