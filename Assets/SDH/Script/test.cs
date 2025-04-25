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
        rb.linearVelocityX = x*15f;
        rb.linearVelocityY = y*15f;
        tf.position = new Vector3(tf.position.x, tf.position.y, tf.position.y);
    }
}
