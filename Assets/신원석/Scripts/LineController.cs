using UnityEngine;


public class LineController : MonoBehaviour
{
    private LineRenderer lineRenderer;
    private RectTransform[] points;

    private void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created


    public void SetUpLine(RectTransform[] points)
    {
        lineRenderer.positionCount = points.Length;
        this.points = points;
    }

    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < points.Length; i++)
        {
            lineRenderer.SetPosition(i, points[i].position);
        }
    }
}
