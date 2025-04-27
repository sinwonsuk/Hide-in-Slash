using Photon.Pun;
using System;
using UnityEngine;

public class SpawnPlayerProfile : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        foreach (var p in PhotonNetwork.PlayerList)
        {
            if (!p.IsLocal) // <-- 이게 중요!
            {
                MakeProfileSlot(p);
            }
        }
    }
   // Update is called once per frame
    void Update()
    {
        
    }

    public void MakeProfileSlot(Photon.Realtime.Player targetPlayer)
    {
        GameObject slot = Instantiate(profileSlotPrefab, profileSlotParent);
        //OtherPlayerProfile profile = slot.GetComponent<OtherPlayerProfile>();
        //profile.SetPlayer(targetPlayer);
    }
}
