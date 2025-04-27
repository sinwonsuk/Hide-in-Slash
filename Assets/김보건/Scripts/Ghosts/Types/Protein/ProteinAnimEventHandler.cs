using UnityEngine;

public class ProteinAnimEventHandler : MonoBehaviour
{
    public Protein protein;

    public void OnProteinAnimationEnd()
    {
        GetComponentInParent<Protein>().EndDrinkProtein();
    }

    public void OnProteinReleaseAnimationEnd()
    {
        GetComponentInParent<Protein>().EndProteinRelease();
    }
}
