using UnityEngine;

public class content : MonoBehaviour
{
    public Transform roomListContent; //��ũ�� ����Ʈ
    void Awake()
    {
        GameReadyManager.Instance.Gc(roomListContent);
    }


}
