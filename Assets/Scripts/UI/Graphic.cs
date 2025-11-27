using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class Graphic : MonoBehaviour
{
    [Header("UI Components")]
    public TMP_Dropdown resolutionDropdown;
    public TMP_Dropdown fpsDropdown;
    public Toggle fullscreenToggle;

    private Resolution[] resolutions;
    private List<Resolution> filteredResolutions;

    private int selectedResolutionIndex;
    private int selectedFpsIndex;
    private bool isFullscreenMode;

    void Start()
    {
        isFullscreenMode = Screen.fullScreen;

        InitResolutions();
        InitFullscreen();
        InitFrameRate();
    }

    void InitResolutions()
    {
        resolutions = Screen.resolutions;
        filteredResolutions = new List<Resolution>();

        resolutionDropdown.ClearOptions();
        float currentRefreshRate = (float)Screen.currentResolution.refreshRateRatio.value;

        List<string> options = new List<string>();

        HashSet<string> addedOptions = new HashSet<string>();
        int currentResIndex = 0;

        for (int i = 0; i < resolutions.Length; i++)
        {
            float resRefreshRate = (float)resolutions[i].refreshRateRatio.value;

            if (Mathf.Abs(resRefreshRate - currentRefreshRate) < 1f && resolutions[i].width >= 1200)
            {
                string option = resolutions[i].width + " x " + resolutions[i].height;
                if (!addedOptions.Contains(option))
                {
                    addedOptions.Add(option);
                    filteredResolutions.Add(resolutions[i]);
                    options.Add(option);

                    if (resolutions[i].width == Screen.width && resolutions[i].height == Screen.height)
                        currentResIndex = filteredResolutions.Count - 1;
                }

                if (resolutions[i].width == Screen.width &&
                    resolutions[i].height == Screen.height)
                {
                    currentResIndex = filteredResolutions.Count - 1;
                }
            }
        }

        resolutionDropdown.AddOptions(options);
        resolutionDropdown.value = currentResIndex;
        resolutionDropdown.RefreshShownValue();

        selectedResolutionIndex = currentResIndex;
    }

    void InitFrameRate()
    {
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 60;
        fpsDropdown.value = 1;
        selectedFpsIndex = 1;
    }

    void InitFullscreen()
    {
        fullscreenToggle.isOn = isFullscreenMode;
    }

    public void OnResolutionChanged(int index)
    {
        selectedResolutionIndex = index;
    }

    public void OnFpsChanged(int index)
    {
        selectedFpsIndex = index;
    }

    public void OnFullscreenToggled(bool isFull)
    {
        isFullscreenMode = isFull;
    }


    public void ApplyGraphicsSettings()
    {
        Resolution resolution = filteredResolutions[selectedResolutionIndex];
        Screen.SetResolution(resolution.width, resolution.height, isFullscreenMode);

        switch (selectedFpsIndex)
        {
            case 0: Application.targetFrameRate = 30; break;
            case 1: Application.targetFrameRate = 60; break;
            case 2: Application.targetFrameRate = 144; break;
            case 3: Application.targetFrameRate = -1; break;
        }

        Debug.Log("그래픽 설정이 적용되었습니다.");
    }
}