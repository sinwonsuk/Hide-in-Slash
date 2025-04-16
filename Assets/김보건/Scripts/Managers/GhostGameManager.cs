using UnityEngine;

public enum GhostType
{
    Peanut = 0,
    Protein = 1,
    PukeGirl = 2
}

public class GhostGameManager : MonoBehaviour
{
    [Header("�ͽ�")]
    public GameObject peanutPrefab;
    public GameObject proteinPrefab;
    public GameObject pukeGirlPrefab;

    [Header("�ͽ� ���� ��ġ")]
    public Transform spawnPoint;


    void Start()
    {
        SpawnRandomGhost();
    }

    void SpawnRandomGhost()
    {
        GhostType randomType = (GhostType)Random.Range(0, 3);
        GameObject Ghost = null;

        switch (randomType)
        {
            case GhostType.Peanut:
                Ghost = peanutPrefab;
                break;
            case GhostType.Protein:
                Ghost = proteinPrefab;
                break;
            case GhostType.PukeGirl:
                Ghost = pukeGirlPrefab;
                break;
        }

        Instantiate(Ghost, spawnPoint.position, Quaternion.identity);
        Debug.Log($"[{randomType}] �ͽ� ����");
        
    }
}
