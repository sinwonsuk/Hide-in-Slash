using NUnit.Framework;
using Photon.Pun;
using System.Collections.Generic;
using UnityEngine;

public class GeneratorManager : MonoBehaviourPunCallbacks
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        EventManager.RegisterEvent(EventType.GeneratorSuccess, addCount);
    }

    // Update is called once per frame
    void Update()
    {

        if (generatorCount == 0 && isCheck ==false && PhotonNetwork.IsMasterClient)
        {
            photonView.RPC("BroadcastSuecss", RpcTarget.All);       
        }
    }

    [PunRPC]
    public void BroadcastSuecss()
    {
        fadeout.SetActive(true);
        EventManager.TriggerEvent(EventType.AllGeneratorSuccess);
        isCheck = true;
    }

    public void addCount()
    {
        generatorCount++;
    }

    bool isCheck = false;

    int generatorCount = 0;

    [SerializeField]
    GameObject fadeout;

}
