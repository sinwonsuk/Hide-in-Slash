using Photon.Pun;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class generatorGage : MonoBehaviour
{

    void Start()
    {
        generator = GetComponentInParent<Generator>();

    }

    // Update is called once per frame
    void Update()
    {        
        generatorInImage.fillAmount += Time.deltaTime * speed;

        if (generatorInImage.fillAmount == 1)
        {
            generatorInImage.enabled = false;
            Destroy(gameObject);
            generator.DeleteAction.Invoke();
        }
    }

    [SerializeField]
    float speed = 0.5f;

    public Image generatorInImage;

    Generator generator;
}
