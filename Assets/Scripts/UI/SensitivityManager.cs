using UnityEngine;
using UnityEngine.UI;

public class SensitivityManager : MonoBehaviour
{
    public ThirdPersonPlayerController playerController;
    public Slider sensitivitySlider;

    private const string SensitivityKey = "MouseSensitivity";
    private float defaultSensitivity = 1f;

    void Start()
    {
        float savedSensitivity = PlayerPrefs.GetFloat(SensitivityKey, defaultSensitivity);

        if (playerController != null)
        {
            playerController.mouseSensitivity = savedSensitivity;
        }

        if(sensitivitySlider != null)
        {
            sensitivitySlider.value = playerController.mouseSensitivity;
        }
    }

    public void OnSensitivityChanged(float newValue)
    {
        Debug.Log("�����̴� �� �����: " + newValue);
        if (playerController != null)
        {
            playerController.mouseSensitivity = newValue;
        }
        PlayerPrefs.SetFloat(SensitivityKey, newValue);
        PlayerPrefs.Save();
    }
}