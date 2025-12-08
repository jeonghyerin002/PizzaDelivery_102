using UnityEngine;
using UnityEngine.UI;

public class VolumeController : MonoBehaviour
{
    [Header("참조")]
    public Slider masterSlider;
    public Slider bgmSlider;
    public Slider sfxSlider;

    void Start()
    {
        //값 불러오기
        float masterVol = PlayerPrefs.GetFloat("Master_Vol", 0.5f);
        float bgmVol = PlayerPrefs.GetFloat("BGM_Vol", 0.5f);
        float sfxVol = PlayerPrefs.GetFloat("SFX_Vol", 0.5f);

        //위치 맞추기
        if (masterSlider != null)
        {
            masterSlider.value = masterVol;
            masterSlider.onValueChanged.AddListener(AudioManager.instance.SetMasterVolume);
        }

        if (bgmSlider != null)
        {
            bgmSlider.value = bgmVol;
            bgmSlider.onValueChanged.AddListener(AudioManager.instance.SetBGMVolume);
        }

        if (sfxSlider != null)
        {
            sfxSlider.value = sfxVol;
            sfxSlider.onValueChanged.AddListener(AudioManager.instance.SetSFXVolume);
        }
    }
}