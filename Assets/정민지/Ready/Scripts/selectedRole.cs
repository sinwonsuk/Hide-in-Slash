using UnityEngine;

public class selectedRole : MonoBehaviour
{
    [SerializeField] private GameObject playerRole;
    [SerializeField] private GameObject bossRole;

    public void OnClickPlayerRole()
    {
        playerRole.SetActive(true);
        bossRole.SetActive(false);
    }

    public void OnClickBossRole()
    {
        bossRole.SetActive(true);
        playerRole.SetActive(false);
    }


}
