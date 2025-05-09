using UnityEngine;

public class SoundSettingUIExit : MonoBehaviour
{
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)&& gameObject.activeSelf == true)
        {
            Destroy(gameObject);
        }
    }

    public void OnClickSoundSettingExit()
    {
        Destroy(gameObject);
    }
}
