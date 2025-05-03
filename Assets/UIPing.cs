using System.Collections;
using UnityEngine;

public class UIPing : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartCoroutine(Init());
    }
    IEnumerator Init()
    {
        gameObject.SetActive(true); // PhotonView 등록 유도

        yield return null; // 한 프레임 대기
                           // 
        gameObject.SetActive(false); // 안전하게 비활성화
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
