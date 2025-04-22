using UnityEngine;

public class escAndTimeScaleIsOne : MonoBehaviour
{
    public void OnClick()
    {
        Destroy(gameObject);
        Time.timeScale = 1f;
    }
}
