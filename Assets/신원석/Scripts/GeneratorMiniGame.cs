using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

public class GeneratorMiniGame : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GameObject instantiate = Instantiate(generatorStopObject, transform);

        float posX = Random.Range(-1.44f, 0.88f);
        float posY = 1.47f;

        instantiate.transform.localPosition = new Vector2(posX, posY);

        generatorStopObject = instantiate;
      
        stopSquareCheckAction = CheckCollsion;
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyUp(KeyCode.E))
        {          
           Destroy(gameObject);            
        }
    }

    void CheckCollsion()
    {
        IsCheck = true;

        GameObject GeneratorGage = GameObject.Find("GeneratorGage");

        generatorGage generatorGage = GeneratorGage.GetComponent<generatorGage>();

        generatorGage.generatorInImage.fillAmount += 0.2f;

    }

   public Action stopSquareCheckAction;

    public bool IsCheck { get; set; } = false;

    [SerializeField]
    GameObject generatorStopObject;

}
