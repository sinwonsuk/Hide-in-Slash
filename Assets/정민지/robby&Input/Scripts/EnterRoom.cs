using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;

public class EnterRoom : MonoBehaviour
{
    public TMP_InputField passwordInputField;
    private string selectedRoomName;

    public void SetRoomName(string roomName)
    {
        selectedRoomName = roomName;
        Debug.Log($"[SetRoomName] Room name set to: {selectedRoomName}");
    }

    public void OnJoinButtonClicked()
    {
        if (string.IsNullOrEmpty(selectedRoomName))
        {
            Debug.LogError("Room name is null or empty.");
            return;
        }

        RoomInfo selectedRoom = GameReadyManager.Instance.GetRoomByName(selectedRoomName);
        if (selectedRoom != null)
        {
            string enteredPassword = passwordInputField.text;
            GameReadyManager.Instance.TryJoinRoom(selectedRoom, enteredPassword);
        }
    }

    void Update()
    {
        if(passwordInputField!=null&& Input.GetKeyDown(KeyCode.Return))
        {
            OnJoinButtonClicked();
        }
    }
}
