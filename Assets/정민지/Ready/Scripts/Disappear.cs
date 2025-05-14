using UnityEngine;

public class Disappear : MonoBehaviour
{
    public void OnClickVisibleObject()
    {
        gameObject.SetActive(false);
    }
}
