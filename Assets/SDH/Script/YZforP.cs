using UnityEngine;

public class YZforP : MonoBehaviour
{
    void Update()
    {
        transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.y-0.75f);
    }
}
