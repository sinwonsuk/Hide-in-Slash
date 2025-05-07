using UnityEngine;

public class FlashLightY : MonoBehaviour
{
    [SerializeField] private GameObject player;
    void Update()
    {
        transform.position = new Vector3(transform.position.x, transform.position.y, player.transform.position.y);
    }
}
