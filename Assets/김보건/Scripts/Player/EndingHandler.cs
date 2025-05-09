using System.Collections;
using UnityEngine;
using Photon.Pun;

public class EndingHandler : MonoBehaviour
{
    public void StartEndSequence(GameObject ui, float sec)
    {
        DontDestroyOnLoad(this); // 씬 넘어가도 살아 있게
        StartCoroutine(Run(ui, sec));
    }

    private IEnumerator Run(GameObject ui, float sec)
    {
        yield return new WaitForSeconds(sec);

        if (ui != null)
            Destroy(ui);
        if (PhotonNetwork.InRoom)
            PhotonNetwork.LeaveRoom();

        yield return new WaitForSeconds(0.1f);
        if (!PhotonNetwork.InLobby)
            PhotonNetwork.JoinLobby();

        Destroy(gameObject); // 다 끝났으면 이 오브젝트도 제거
    }
}

