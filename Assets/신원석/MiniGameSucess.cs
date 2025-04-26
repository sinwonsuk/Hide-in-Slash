using UnityEngine;

public class MiniGameSucess : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        Vector2 camera = Camera.main.transform.position;

        transform.position = camera;

        color = Color.white;
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 camera = Camera.main.transform.position;

        transform.position = camera;



        if (color.a > 0f)
        {
            color.a -= Time.deltaTime * speed; 
            spriteRenderer.color = color;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    SpriteRenderer spriteRenderer;
    Color color;
    [SerializeField]
    float speed = 0.5f;


}
