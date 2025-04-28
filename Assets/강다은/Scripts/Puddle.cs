using Photon.Realtime;
using System.Collections;
using UnityEngine;

public class Puddle : MonoBehaviour
{
	[Range(0.1f, 1f)] public float slowFactor = 0.5f;

    [Tooltip("밟았을 때 느려지는 시간(초)")]
    [SerializeField]
    private float slowDuration = 5f;

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (!col.CompareTag("Player"))
            return;

        var player = col.GetComponent<Player>();
        if (player != null)
            StartCoroutine(ApplySlowTemporarily(player));
    }

    private IEnumerator ApplySlowTemporarily(Player player)
    {
        // 1) 속도 느려지기
        player.ApplySlow(slowFactor);

        // 2) slowDuration초만큼 대기
        yield return new WaitForSeconds(slowDuration);

        // 3) 원래 속도로 복귀
        player.ResetSpeed();
    }
}
