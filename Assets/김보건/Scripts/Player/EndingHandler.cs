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
        {
            PhotonNetwork.LeaveRoom();

            // LeaveRoom()은 비동기 → OnLeftRoom()을 기다려야 안정적
            while (PhotonNetwork.InRoom)
                yield return null;
        }

        Destroy(gameObject); // 이 오브젝트 제거
    }
}

