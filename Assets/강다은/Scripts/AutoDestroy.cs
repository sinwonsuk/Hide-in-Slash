using UnityEngine;

public class AutoDestroy : MonoBehaviour
{
    private float destroyTime = 2f;

    void Start()
    {
        if(CompareTag("Dice"))
            Destroy(gameObject, destroyTime);
    }

}
