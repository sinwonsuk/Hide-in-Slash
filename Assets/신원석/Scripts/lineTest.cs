using UnityEngine;

public class lineTest : MonoBehaviour
{
    [SerializeField]
    private RectTransform[] points;
    [SerializeField]
    private LineController line;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        line.SetUpLine(points);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
