using UnityEngine;

public class ProteinAnimEventHandler : MonoBehaviour
{
    public Protein protein;

    public void OnProteinAnimationEnd()
    {
        GetComponentInParent<Protein>().EndDrinkProtein();
    }
}
