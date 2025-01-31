using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class Settings : MonoBehaviour
{
    [SerializeField] List<KeybindData> keybindDataList = new List<KeybindData>();

    [SerializeField] private TMP_Dropdown resolutionDropdown;
    [SerializeField] private TMP_Dropdown windowModeDropdown;
    [SerializeField] private TMP_Dropdown primaryDisplayDropdown;
    [SerializeField] private TMP_Dropdown qualityDropdown;
    [SerializeField] private TMP_Dropdown targetFramerateDropdown;
    [SerializeField] private Slider masterVolumeSlider;
    [SerializeField] private Slider soundVolumeSlider;
    [SerializeField] private Slider musicVolumeSlider;

    private List<Resolution> resolutions = new List<Resolution>();
    private List<Display> displays = new List<Display>();
    private List<QualitySettings> qualitySettings = new List<QualitySettings>();

    static List<int> targetFramerates = new List<int> { 30, 60, 120, 160, 180, 240 };
    static List<string> windowModeLabels = new List<string> { "Fullscreen", "Borderless", "Maximized window", "Window" };
    static List<FullScreenMode> windowModes = new List<FullScreenMode> { FullScreenMode.ExclusiveFullScreen, FullScreenMode.FullScreenWindow, FullScreenMode.MaximizedWindow, FullScreenMode.Windowed };

    [SerializeField] private GameObject keybindPrefab;
    [SerializeField] private GameObject keybindPrefabContainer;
    [SerializeField] public static GameObject keybindChangeMenu;

    // Start is called before the first frame update
    void Start()
    {
        InitializeKeybindsMenu();
        Refresh();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void InitializeKeybindsMenu()
    {
        if (keybindPrefab == null || keybindPrefabContainer == null)
        {
            Debug.LogWarning("KeybindPrefab or KeybindPrefabContainer is not set");
            return;
        }

        foreach (KeybindData keybindData in keybindDataList)
        {
            if (keybindData.action == ActionType.None) continue;
            GameObject instance = Instantiate(keybindPrefab, keybindPrefabContainer.transform);
            KeybindPrefab instanceScript = instance.GetComponent<KeybindPrefab>();
            if (instanceScript == null)
            {
                Debug.LogError("KeybindPrefab script not found");
                return;
            }
            instanceScript.keybindData = keybindData;
            // TODO?
        }
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
        int currentResolution = -1;
        List<string> options = new List<string>();

        for (int i = availableResolutions.Length - 1; i >= 0; i--)
        {
            Resolution resolution = availableResolutions[i];
            resolutions.Add(resolution);
            options.Add(resolution.width + "x" +  resolution.height);
            if (Screen.currentResolution.width == resolution.width && Screen.currentResolution.height == resolution.height)
            {
                currentResolution = (availableResolutions.Length - 1) - i;
            }
        }

        resolutionDropdown.ClearOptions();
        resolutionDropdown.AddOptions(options);
        resolutionDropdown.RefreshShownValue();
        resolutionDropdown.value = currentResolution;

        // Window Mode
        int currentFullscreenMode = 0;

        for (int i = 0; i < windowModes.Count; i++)
        {
            if (Screen.fullScreenMode == windowModes[i])
            {
                currentFullscreenMode = i;
            }
        }

        windowModeDropdown.ClearOptions();
        windowModeDropdown.AddOptions(windowModeLabels);
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

        qualityDropdown.ClearOptions();
        qualityDropdown.AddOptions(options);
        qualityDropdown.value = QualitySettings.GetQualityLevel();
        qualityDropdown.RefreshShownValue();

        // Target Framerate
        int currentFramerate = Application.targetFrameRate;
        int targetFramerateIndex = -1;
        options = new List<string>();
        for (int i = 0; i < targetFramerates.Count; i++)
        {
            options.Add(targetFramerates[i] + " FPS");
            if (targetFramerates[i] == currentFramerate)
            {
                targetFramerateIndex = i;
            }
        }
        if (targetFramerateIndex == -1)
        {
            if (currentFramerate != -1)
            {
                Debug.LogError("Target Framerate not found");
                return;
            }
            options.Add("Unlimited");
            targetFramerateIndex = options.Count - 1;
        }
        targetFramerateDropdown.ClearOptions();
        targetFramerateDropdown.AddOptions(options);
        targetFramerateDropdown.value = targetFramerateIndex;
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
