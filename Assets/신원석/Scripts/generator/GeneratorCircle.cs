using UnityEngine;

public class GeneratorCircle : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        miniGame = GetComponentInParent<GeneratorMiniGame>();
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(Vector2.right * speed * Time.deltaTime);

        if(isCheck ==true && Input.GetKeyDown(KeyCode.G))
        {
            miniGame.stopSquareCheckAction.Invoke();
        }
        else if(Input.GetKeyDown(KeyCode.G))
        {
            miniGame.stopSquareCheckFlaseAction.Invoke();
        }
          
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("generatorMiniGameWall"))
        {
            speed *= -1;
        }

        if(collision.CompareTag("generatorMiniGameStop"))
        {
            isCheck = true;
        }
        
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("generatorMiniGameStop"))
        {
            isCheck = false;
        }
    }


    GeneratorMiniGame miniGame;

    bool isCheck = false;

    [SerializeField]
    float speed = 1.0f;
}
