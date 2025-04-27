using UnityEngine;

public class test : MonoBehaviour
{
    Transform tf;
    Rigidbody2D rb;
    float x;
    float y;
    private void Start()
    {
        tf = transform;
        rb = GetComponent<Rigidbody2D>();
    }
    private void Update()
    {
        x = Input.GetAxisRaw("Horizontal");
        y = Input.GetAxisRaw("Vertical");
        rb.linearVelocityX = x*5f;
        rb.linearVelocityY = y*5f;
        tf.position = new Vector3(tf.position.x, tf.position.y, tf.position.y);
        if(Input.GetKeyDown(KeyCode.Space))
        {
            tf.position = new Vector3(50, 0, 0);
        }
    }
}
