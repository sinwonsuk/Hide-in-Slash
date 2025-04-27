using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class RegionTrigger : MonoBehaviour
{
	[Tooltip("Region Name")]
	public string regionName;

	private readonly HashSet<GameObject> survivorsInside = new();
	public bool HasAnySurvivor => survivorsInside.Count > 0;

	private void OnTriggerEnter2D(Collider2D other)
	{
		if (other.CompareTag("Player"))
			survivorsInside.Add(other.gameObject);
	}

	private void OnTriggerExit2D(Collider2D other)
	{
		if (other.CompareTag("Player"))
			survivorsInside.Remove(other.gameObject);
	}
}
