using UnityEngine;

public class content : MonoBehaviour
{
    public Transform roomListContent; //스크롤 콘텐트
    void Awake()
    {
        GameReadyManager.Instance.Gc(roomListContent);
    }


}
