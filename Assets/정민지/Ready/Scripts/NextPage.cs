using UnityEngine;

public class NextPage : MonoBehaviour
{
    [SerializeField] private GameObject NewPage;
    [SerializeField] private GameObject CurrentPage;

    public void OnClickNext()
    {
        CurrentPage.SetActive(false);
        NewPage.SetActive(true);
    }
}
