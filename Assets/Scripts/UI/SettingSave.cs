// (파일 이름 예: TitleSettingsManager.cs)

using UnityEngine;
using UnityEngine.UI;

public class SettingSave : MonoBehaviour
{
    public Slider sensitivitySlider;

    private const string SensitivityKey = "MouseSensitivity";
    private float defaultSensitivity = 0.3f;

    void Start()
    {
        //PlayerPrefs에서 저장된 값을 불러와서 슬라이더에 표시
        float savedSensitivity = PlayerPrefs.GetFloat(SensitivityKey, defaultSensitivity);

        if (sensitivitySlider != null)
        {
            sensitivitySlider.value = savedSensitivity;
        }
    }

    //슬라이더 값이 변경되면 PlayerPrefs에 저장
    public void OnSensitivityChanged(float newValue)
    {
        PlayerPrefs.SetFloat(SensitivityKey, newValue);
        PlayerPrefs.Save();
    }
}