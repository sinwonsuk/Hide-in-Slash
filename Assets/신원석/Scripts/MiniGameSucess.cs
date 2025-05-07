using UnityEngine;
using UnityEngine.UI;

public class MiniGameSucess : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        SoundManager.GetInstance().SfxPlay(SoundManager.sfx.MiniGmaeSucess, false);

        spriteRenderer = GetComponent<Image>();
     
        color = Color.white;
    }

    // Update is called once per frame
    void Update()
    {
       
        if (color.a > 0f)
        {
            color.a -= Time.deltaTime * speed; 
            spriteRenderer.color = color;
        }
        else
        {
            Destroy(transform.parent.gameObject);
        }
    }

    Image spriteRenderer;
    Color color;
    [SerializeField]
    float speed = 0.5f;


}
