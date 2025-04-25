using UnityEngine;
using UnityEngine.UI;

public class generatorGage : MonoBehaviour
{

    private void OnEnable()
    {

    }
    private void OnDisable()
    {

    }


    void Start()
    {
        generatorInImage = GetComponent<Image>();
    }

    // Update is called once per frame
    void Update()
    {        
        generatorInImage.fillAmount += Time.deltaTime * speed;

        if (generatorInImage.fillAmount == 1)
        {

        }

    }


    [SerializeField]
    float speed = 0.5f;

    public int asd;

    public Image generatorInImage;
}
