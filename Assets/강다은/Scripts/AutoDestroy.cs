using UnityEngine;

public class AutoDestroy : MonoBehaviour
{
    private float destroyTime = 2f;

    void Start()
    {
        Destroy(gameObject, destroyTime);
    }

}
