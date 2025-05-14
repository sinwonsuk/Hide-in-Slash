using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;
using System.Linq;
using System.Collections.Generic;
using ExitGames.Client.Photon; // CustomProperties를 다루기 위해 필요

public class roomClick : MonoBehaviour
{
    public GameObject passwordPanelPrefab;  // 프리팹 (Hierarchy에 없어야 함)
    [SerializeField] private TMP_Text roomName; //방이름


    public void OnRoomButtonClicked()
    {
        SoundManager.GetInstance().SfxPlay(SoundManager.sfx.SfxSetting, false);
        string selectedRoomName = roomName.text;

        RoomInfo getRoom = GameReadyManager.Instance.GetRoomByName(selectedRoomName); //방이름으로 방정보 가져오기

        if (getRoom.CustomProperties.ContainsKey("pw") && !string.IsNullOrEmpty(getRoom.CustomProperties["pw"] as string)) //패스워드가 방정보에 포함되어있거나 비어있지 않을 경우
        {
            GameReadyManager.Instance.instantiatePwprefab(selectedRoomName);

           
        }
        else //패스워드가 비어있다면
        {
            PhotonNetwork.JoinRoom(getRoom.Name);
        }

    }
}