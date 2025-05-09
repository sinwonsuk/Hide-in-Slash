using UnityEngine;
using UnityEngine.UI;

public class SoundSettingsUI : MonoBehaviour
{
    public Slider bgmSlider;
    public Slider sfxSlider;

    private void Start()
    {
        // PlayerPrefs에서 저장된 값 불러오기
        float savedBgm = PlayerPrefs.GetFloat("BGMVolume", 0.5f);
        float savedSfx = PlayerPrefs.GetFloat("SFXVolume", 0.5f);

        // 슬라이더에 값 반영
        bgmSlider.value = savedBgm;
        sfxSlider.value = savedSfx;

        // SoundManager에 값 반영
        SoundManager.GetInstance().SetSoundBgm(savedBgm);
        SoundManager.GetInstance().bgmVolume = savedBgm;
        SoundManager.GetInstance().sfxVolume = savedSfx;

        // 슬라이더 리스너 연결
        bgmSlider.onValueChanged.AddListener(SetBGMVolume);
        sfxSlider.onValueChanged.AddListener(SetSFXVolume);
    }

    public void SetBGMVolume(float value)
    {
        SoundManager.GetInstance().SetSoundBgm(value);
        SoundManager.GetInstance().bgmVolume = value;
        PlayerPrefs.SetFloat("BGMVolume", value);
        PlayerPrefs.Save();
    }

    public void SetSFXVolume(float value)
    {
        SoundManager manager = SoundManager.GetInstance();
        manager.sfxVolume = value;
        manager.UpdateSfxVolumes();

        PlayerPrefs.SetFloat("SFXVolume", value);
        PlayerPrefs.Save();
    }
}
