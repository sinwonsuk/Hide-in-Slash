using Photon.Pun;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class generatorGage : MonoBehaviourPunCallbacks
{

    void Start()
    {
        generator = GetComponentInParent<Generator>();
        pv = generator.GetComponent<PhotonView>();
    }

    // Update is called once per frame
    void Update()
    {


        pv.RPC("AddGage", RpcTarget.MasterClient, Time.deltaTime / durationTime);


        if (generatorInImage.fillAmount >= 1 && isCheck ==false)
        {
            generatorInImage.enabled = false;
            Destroy(gameObject);

            if (PhotonNetwork.IsConnected && !PhotonNetwork.IsMasterClient)
            {
                PhotonView pv = generator.GetComponent<PhotonView>();
                if (pv != null)
                {
                    pv.RPC("RequestGeneratorComplete", RpcTarget.MasterClient);
                }
            }
            // 마스터 클라일 경우 직접 호출
            else if (PhotonNetwork.IsMasterClient)
            {
                generator.DeleteAction.Invoke();
            }

            isCheck = true;
        }


        
    }

    public PhotonView pv { get; private set; }
    bool isCheck = false;
    
    [SerializeField]
    float speed = 0.5f;

    public Image generatorInImage;

    Generator generator;

    float durationTime = 30.0f;
}
