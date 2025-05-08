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

        float posX = Random.Range(-1.57f, 0.98f);
        float posY = 1.45f;

        instantiate.transform.localPosition = new Vector2(posX, posY);

        generatorStopObject = instantiate;
      
        stopSquareCheckAction = CheckCollsion;
        stopSquareCheckFlaseAction = NotCheckCollsion;
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

       

        if(GeneratorGage == null)
        {           
            return;
        }
      
        GeneratorGage.pv.RPC("AddGage", Photon.Pun.RpcTarget.MasterClient, +0.05f);

    }
    void NotCheckCollsion()
    {
        IsCheck = true;

        if (GeneratorGage == null)
        {
            return;
        }

        GeneratorGage.pv.RPC("AddGage", Photon.Pun.RpcTarget.MasterClient, -0.05f);


    }



    public Action stopSquareCheckAction;
    public Action stopSquareCheckFlaseAction;

    public bool IsCheck { get; set; } = false;

    [SerializeField]
    GameObject generatorStopObject;

    public generatorGage GeneratorGage { get; set; }

}
