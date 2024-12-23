using System.Collections;
using System.Collections.Generic;
//using UnityEditor.Search;
using UnityEngine;
using UnityEngine.UI;

public class Settings : MonoBehaviour
{
    [SerializeField] private Dropdown resolutionDropdown;
    [SerializeField] private Dropdown windowModeDropdown;
    [SerializeField] private Dropdown primaryDisplayDropdown;
    [SerializeField] private Dropdown qualityDropdown;
    [SerializeField] private Dropdown targetFramerateDropdown;
    [SerializeField] private Slider masterVolumeSlider;
    [SerializeField] private Slider soundVolumeSlider;
    [SerializeField] private Slider musicVolumeSlider;

    private List<Resolution> resolutions = new List<Resolution>();
    private List<FullScreenMode> windowModes = new List<FullScreenMode>();
    private List<Display> displays = new List<Display>();
    private List<QualitySettings> qualitySettings = new List<QualitySettings>();
    private List<int> targetFramerates = new List<int>();

    // Start is called before the first frame update
    void Start()
    {
        // Set values into lists
        windowModes.Clear();
        windowModes.Add(FullScreenMode.ExclusiveFullScreen);
        windowModes.Add(FullScreenMode.FullScreenWindow);
        windowModes.Add(FullScreenMode.Windowed);
        targetFramerates.Clear();
        targetFramerates.Add(30);
        targetFramerates.Add(60);
        targetFramerates.Add(120);
        targetFramerates.Add(180);
        targetFramerates.Add(240);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ResetToDefault()
    {

    }

    // Loads current values into UI elements
    public void Refresh()
    {
        // Resolutions
        resolutions.Clear();
        Resolution[] availableResolutions = Screen.resolutions;
        int currentResolution = 0;
        List<string> options = new List<string>();

        for (int i = 0; i < availableResolutions.Length; i++)
        {
            Resolution resolution = availableResolutions[i];
            resolutions.Add(resolution);
            options.Add(resolution.width + "x" +  resolution.height);
            if (Screen.currentResolution.width == resolution.width && Screen.currentResolution.height == resolution.height)
            {
                currentResolution = i;
            }
        }

        resolutionDropdown.AddOptions(options);
        resolutionDropdown.value = currentResolution;
        resolutionDropdown.RefreshShownValue();

        // Window Mode
        int currentFullscreenMode = 0;

        for (int i = 0; i < windowModes.Count; i++)
        {
            if (Screen.fullScreenMode == windowModes[i])
            {
                currentFullscreenMode = i;
            }
        }

        windowModeDropdown.value = currentFullscreenMode;
        windowModeDropdown.RefreshShownValue();

        // Primary Display
        Display[] availableDisplays = Display.displays;
        options = new List<string>();
        int currentDisplay = 0;

        for (int i = 0; i < availableDisplays.Length; i++)
        {
            Display display = availableDisplays[i];
            displays.Add(display);
            options.Add("Display " + (i + 1));

            if (display.active)
            {
                currentDisplay = i;
            }
        }

        primaryDisplayDropdown.AddOptions(options);
        primaryDisplayDropdown.value = currentDisplay;
        primaryDisplayDropdown.RefreshShownValue();

        // Quality
        string[] qualityLevelNames = QualitySettings.names;
        options = new List<string>();

        for (int i = 0;i < qualityLevelNames.Length; i++)
        {
            options.Add(qualityLevelNames[i]);
        }

        qualityDropdown.AddOptions(options);
        qualityDropdown.value = QualitySettings.GetQualityLevel();
        qualityDropdown.RefreshShownValue();

        // Target Framerate
        int currentFramerate = Application.targetFrameRate;
        int targetFramerate = 0;
        options = new List<string>();
        for (int i = 0; i < targetFramerates.Count; i++)
        {
            options.Add(targetFramerates[i] + " FPS");
            if (targetFramerates[i] == currentFramerate)
            {
                targetFramerate = i;
            }
        }
        targetFramerateDropdown.AddOptions(options);
        targetFramerateDropdown.value = targetFramerate;
        targetFramerateDropdown.RefreshShownValue();
    }

    public void SetResolution(int index)
    {

    }

    public void SetWindowMode(int index)
    {

    }

    public void SetPrimaryDisplay(int index)
    {

    }

    public void SetQuality(int index)
    {

    }

    public void SetTargetFramerate(int index)
    {

    }

    public void SetOutputDevice(int index)
    {

    }

    public void SetMasterVolume(float value)
    {

    }

    public void SetSoundVolume(float value)
    {

    }

    public void SetMusicVolume(float value)
    {

    }
}
