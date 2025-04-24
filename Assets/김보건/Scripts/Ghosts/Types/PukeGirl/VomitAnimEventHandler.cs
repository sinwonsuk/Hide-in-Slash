using UnityEngine;

public class VomitAnimEventHandler : MonoBehaviour
{
    public PukeGirl pukeGirl;

    public void OnVomitAnimationEnd()
    {
        GetComponentInParent<PukeGirl>().OnVomitAnimEnd();
    }
}
