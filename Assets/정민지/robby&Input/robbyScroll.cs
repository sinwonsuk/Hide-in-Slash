using UnityEngine;
using TMPro;

public class robbyScroll : MonoBehaviour
{
    public GameObject itemPrefab; // �̸� ���� ������ (Text�� ��ư ��)
    public Transform contentPanel; // Content ������Ʈ

    void AddItem(string text)
    {
        GameObject item = Instantiate(itemPrefab, contentPanel);
        item.GetComponentInChildren<Text>().text = text;
    }
}
