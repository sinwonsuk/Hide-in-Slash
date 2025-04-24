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
    }


    [SerializeField]
    float speed = 0.5f;

    Image generatorInImage;
}
