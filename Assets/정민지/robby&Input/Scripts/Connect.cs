using UnityEngine;

public class Connect : MonoBehaviour
{
    [SerializeField] private GameObject Manager;
    void Awake()
    {
        Instantiate(Manager);
    }
}
