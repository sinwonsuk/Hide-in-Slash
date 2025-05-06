using TMPro;
using UnityEngine;

public class StoreWindowMove : MonoBehaviour
{
    private void OnEnable()
    {

    }
    private void OnDisable()
    {
        isMove = false;
        uiElement.transform.position = firstPosition;
    }

    void Start()
    {
        SoundManager.GetInstance().SfxPlay(SoundManager.sfx.OpenStore, false); // 사운드 재생
        uiElement = GetComponent<RectTransform>();
        firstPosition = uiElement.anchoredPosition;
        targetPosition = new Vector2(0, 60);

    }

    // Update is called once per frame
    void Update()
    {
        if(isMove ==true)
        {
            MoveStoreWindow();
        }
        else
        {
            MoveStoreWindowReverse();
        }
    }

    public void MoveStoreWindowReverse()
    {
        if (uiElement.anchoredPosition.y >= 1002)
        {
            Destroy(gameObject);
            return;
        }
        uiElement.anchoredPosition = Vector2.Lerp(uiElement.anchoredPosition, firstPosition, Time.deltaTime * speed);

    }

    public void MoveStoreWindow()
    {
        if (uiElement.anchoredPosition.y < targetPosition.y)
        {
            return;
        }
        uiElement.anchoredPosition = Vector2.Lerp(uiElement.anchoredPosition, targetPosition, Time.deltaTime * speed);
    }

    public bool isMove { get; set; }= false;

    RectTransform uiElement;
    Vector2 targetPosition;

    Vector2 firstPosition;

    [SerializeField]
    float speed = 5f;
}
