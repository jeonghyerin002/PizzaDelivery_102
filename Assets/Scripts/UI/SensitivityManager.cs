using UnityEngine;
using UnityEngine.UI;

public class SensitivityManager : MonoBehaviour
{
    public PlayerController playerController;
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
            if(playerController != null)
            {
                sensitivitySlider.value = playerController.mouseSensitivity;
            }
        }
    }

    public void OnSensitivityChanged(float newValue)
    {
        Debug.Log("슬라이더 값 변경됨: " + newValue);
        if (playerController != null)
        {
            playerController.mouseSensitivity = newValue;
        }
        PlayerPrefs.SetFloat(SensitivityKey, newValue);
        PlayerPrefs.Save();
    }
}