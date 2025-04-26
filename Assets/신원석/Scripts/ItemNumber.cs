using UnityEngine;
using UnityEngine.UI;

public class ItemNumber : MonoBehaviour
{

    private void Awake()
    {
        imageNumber = GetComponent<Image>();
    }
  
    // Update is called once per frame
    void Update()
    {
        
    }

    public void ChangeImage(int num)
    {
        if (num == 1)
            imageNumber.sprite = Resources.Load<Sprite>("FirstItem");
        if (num == 2)
            imageNumber.sprite = Resources.Load<Sprite>("SecondItem");
        if (num == 3)
            imageNumber.sprite = Resources.Load<Sprite>("ThirdItem");
        if (num == 4)
            imageNumber.sprite = Resources.Load<Sprite>("FourthItem");
        if (num == 5)
            imageNumber.sprite = Resources.Load<Sprite>("FifthItem");
        if (num == 5)
            imageNumber.sprite = Resources.Load<Sprite>("FifthItem");
    }


    Image imageNumber;

}
