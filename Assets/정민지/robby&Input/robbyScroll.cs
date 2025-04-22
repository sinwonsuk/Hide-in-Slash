using UnityEngine;
using TMPro;

public class robbyScroll : MonoBehaviour
{
    public GameObject itemPrefab; // 미리 만든 프리팹 (Text나 버튼 등)
    public Transform contentPanel; // Content 오브젝트

    void AddItem(string text)
    {
        GameObject item = Instantiate(itemPrefab, contentPanel);
        item.GetComponentInChildren<Text>().text = text;
    }
}
