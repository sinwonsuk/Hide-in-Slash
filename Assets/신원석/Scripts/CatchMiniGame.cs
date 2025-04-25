using UnityEngine;
using UnityEngine.Events;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class CatchMiniGame : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Vector3 cameraPos = Camera.main.transform.position;

        transform.position = new Vector3(cameraPos.x, cameraPos.y, 0);
    
        CircleDeleteCheckAction = CheckCircle;

        for (int i = 0; i < CircleCount; i++)
        {
            GameObject instantiate = Instantiate(circle,transform);

            BounceCircle bounce = instantiate.GetComponent<BounceCircle>();

            float posX = Random.Range(-3.35f, 3.3f);
            float posY = Random.Range(-3.35f, 3.3f);

            bounce.transform.localPosition = new Vector2(posX, posY);

        }
    }

    public void CheckCircle()
    {
        check++;

        if(check == CircleCount)
        {
            Debug.Log("미니게임 승리");
        }

    }

    // Update is called once per frame
    void Update()
    {
        
    }
    [SerializeField]
    int CircleCount;

    int check = 0;

    [SerializeField]
    GameObject circle;

    public UnityAction CircleDeleteCheckAction;
}
