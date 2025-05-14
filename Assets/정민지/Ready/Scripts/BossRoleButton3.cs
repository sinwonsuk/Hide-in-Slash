using UnityEngine;

public class BossRoleButton3 : MonoBehaviour
{
    [SerializeField] private GameObject Peanut;
    [SerializeField] private GameObject Mamarote;
    [SerializeField] private GameObject Cancer;

    public void OnClickPeanut()
    {
        Peanut.SetActive(true);
        Mamarote.SetActive(false);
        Cancer.SetActive(false);
    }

    public void OnClickMamarote()
    {
        Peanut.SetActive(false);
        Mamarote.SetActive(true);
        Cancer.SetActive(false);
    }

    public void OnClickCancer()
    {
        Peanut.SetActive(false);
        Mamarote.SetActive(false);
        Cancer.SetActive(true);
    }
}
