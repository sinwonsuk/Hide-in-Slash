using UnityEngine;

public class Puddle : MonoBehaviour
{
	[Range(0.1f, 1f)] public float slowFactor = 0.5f;

	private void OnTriggerEnter2D(Collider2D col)
	{
		if (!col.CompareTag("Player")) return;

		// PlayerMove 컴포넌트를 찾아서 슬로우 적용
		var mover = col.GetComponent<PlayerMove>();
		if (mover != null)
			mover.SetSpeedMultiplier(slowFactor);
	}

	private void OnTriggerExit2D(Collider2D col)
	{
		if (!col.CompareTag("Player")) return;

		var mover = col.GetComponent<PlayerMove>();
		if (mover != null)
			mover.SetSpeedMultiplier(1f);
	}
}
