// (���� �̸� ��: TitleSettingsManager.cs)

using UnityEngine;
using UnityEngine.UI;

public class SettingSave : MonoBehaviour
{
    public Slider sensitivitySlider;

    private const string SensitivityKey = "MouseSensitivity";
    private float defaultSensitivity = 0.3f;

    void Start()
    {
        //PlayerPrefs���� ����� ���� �ҷ��ͼ� �����̴��� ǥ��
        float savedSensitivity = PlayerPrefs.GetFloat(SensitivityKey, defaultSensitivity);

        if (sensitivitySlider != null)
        {
            sensitivitySlider.value = savedSensitivity;
        }
    }

    //�����̴� ���� ����Ǹ� PlayerPrefs�� ����
    public void OnSensitivityChanged(float newValue)
    {
        PlayerPrefs.SetFloat(SensitivityKey, newValue);
        PlayerPrefs.Save();
    }
}