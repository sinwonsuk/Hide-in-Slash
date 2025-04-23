using UnityEngine;

public class BounceCircle : MonoBehaviour
{  
    private void Awake()
    {
        dir= new Vector2(0, 0);
        rigidbody2D = GetComponent<Rigidbody2D>();
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        miniGame = GetComponentInParent<CatchMiniGame>();

        float posX = Random.Range(-1.0f, 1.0f);
        float posY = Random.Range(-1.0f, 1.0f);

        Vector2 moveDir = new Vector2(posX, posY);

        rigidbody2D.linearVelocity = moveDir.normalized * speed;

        dir = rigidbody2D.linearVelocity;

    }
    // Update is called once per frame
    void Update()
    {
        DeleteCircle();
    }

    public void DeleteCircle()
    {
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        Collider2D col = Physics2D.OverlapPoint(mousePos);

        if (col != null && col.gameObject == gameObject)
        {
            if(Input.GetKeyDown(KeyCode.Mouse0))
            {
                Destroy(gameObject);
                miniGame.CircleDeleteCheckAction.Invoke();
            }          
        }
    }


    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("DrawMiniGameWall"))
        {
            Vector2 incomingVelocity = dir;

            Vector2 normal = collision.contacts[0].normal;

            Vector2 reflected = Vector2.Reflect(incomingVelocity, normal);

            rigidbody2D.linearVelocity = reflected.normalized * speed;

            dir = rigidbody2D.linearVelocity;
        }      
    }

    new Rigidbody2D rigidbody2D;
    float speed = 5f;
    Vector2 dir;

    CatchMiniGame miniGame;
}
