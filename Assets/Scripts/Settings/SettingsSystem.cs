using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using static UnityEngine.Rendering.DebugUI;

public class Settings : MonoBehaviour
{
    [SerializeField] private GameObject keybindPrefab;
    [SerializeField] private GameObject keybindPrefabContainer;
    [SerializeField] public static GameObject keybindChangeOverlay;
    [SerializeField] private List<GameObject> tabs = new List<GameObject>();
    [SerializeField] private List<GameObject> panels = new List<GameObject>();

    [SerializeField] List<KeybindData> keybindDataList = new List<KeybindData>();

    [SerializeField] private TMP_Dropdown resolutionDropdown;
    [SerializeField] private TMP_Dropdown windowModeDropdown;

    private List<Resolution> resolutions = new List<Resolution>();
    private List<KeybindPrefab> keybindPrefabs = new List<KeybindPrefab>();

    static List<string> windowModeLabels = new List<string> { "Fullscreen", "Borderless", "Maximized window", "Window" };
    static List<FullScreenMode> windowModes = new List<FullScreenMode> { FullScreenMode.ExclusiveFullScreen, FullScreenMode.FullScreenWindow, FullScreenMode.MaximizedWindow, FullScreenMode.Windowed };

    private int currentTabIndex = 0;

    // Start is called before the first frame update
    void Awake()
    {
        keybindChangeOverlay = transform.GetChild(transform.childCount - 1).gameObject;
        panels.ForEach(panel => panel.SetActive(false));
        if (keybindChangeOverlay != null) keybindChangeOverlay.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Open()
    {
        ChangeTab(0, true);
    }

    public void Close()
    {
        if (panels[1].activeInHierarchy) ApplyKeybindChanges();
    }

    // Possibly add cloud sync
    // This method is called when reloading the keybind menu to load current keybind buttons in use
    private void ReloadKeybindsMenu()
    {
        if (keybindPrefabs.Count > 0)
        {
            keybindPrefabs.ForEach(prefab =>
            {
                Destroy(prefab.gameObject);
            });
            keybindPrefabs.Clear();
        }

        if (keybindPrefab == null || keybindPrefabContainer == null)
        {
            Debug.LogWarning("KeybindPrefab or KeybindPrefabContainer is not set");
            return;
        }

        foreach (KeybindData keybindData in keybindDataList)
        {
            if (keybindData.action == ActionType.None) continue;
            if (keybindData.influence == KeybindInfluence.None)
            {
                Debug.LogError("Keybind influence was not set");
                return;
            }
            GameObject instance = Instantiate(keybindPrefab, keybindPrefabContainer.transform);
            KeybindPrefab instanceScript = instance.GetComponent<KeybindPrefab>();
            if (instanceScript == null)
            {
                Debug.LogError("KeybindPrefab script not found");
                return;
            }
            instanceScript.keybindData = keybindData;
            keybindPrefabs.Add(instanceScript);

            if (keybindData.influence == KeybindInfluence.Positive)
            {
                instanceScript.primaryKeyCode = KeybindManager.instance.keybinds[keybindData.action].positiveKey;
                instanceScript.altKeyCode = KeybindManager.instance.keybinds[keybindData.action].positiveAltKey;
            }else
            {
                instanceScript.primaryKeyCode = KeybindManager.instance.keybinds[keybindData.action].negativeKey;
                instanceScript.altKeyCode = KeybindManager.instance.keybinds[keybindData.action].negativeAltKey;
            }
            instanceScript.label.text = keybindData.label;
        }
    }

    public void ApplyKeybindChanges()
    {
        keybindPrefabs.ForEach(keybind =>
        {
            if (keybind.keybindData.action == ActionType.None) return;
            switch (keybind.keybindData.influence)
            {
                case KeybindInfluence.Positive:
                    KeybindManager.instance.keybinds[keybind.keybindData.action].positiveKey = keybind.primaryKeyCode;
                    KeybindManager.instance.keybinds[keybind.keybindData.action].positiveAltKey = keybind.altKeyCode;
                    break;
                case KeybindInfluence.Negative:
                    KeybindManager.instance.keybinds[keybind.keybindData.action].negativeKey = keybind.primaryKeyCode;
                    KeybindManager.instance.keybinds[keybind.keybindData.action].negativeAltKey = keybind.altKeyCode;
                    break;
                default:
                    Debug.LogError("Keybind influence is not set");
                    break;
            }
        });
        // TODO: apply other changes
    }

    // Resets current tab to default settings
    public void ResetToDefault()
    {
        if (currentTabIndex == 1)
        {
            KeybindManager.instance.LoadDefaultsFromKeybindData();
            ReloadKeybindsMenu();
        }
    }

    // Loads current values into UI elements
    public void ReloadGeneralMenu()
    {
        // Resolutions
        resolutions.Clear();
        Resolution[] availableResolutions = Screen.resolutions;
        int currentResolution = -1;
        List<string> options = new List<string>();

        for (int i = availableResolutions.Length - 1; i >= 0; i--)
        {
            Resolution resolution = availableResolutions[i];
            if (resolutions.Count > 0 && resolution.width == resolutions[resolutions.Count - 1].width && resolution.height == resolutions[resolutions.Count - 1].height) continue;
            resolutions.Add(resolution);
            options.Add(resolution.width + ":" +  resolution.height);
            if (Screen.currentResolution.width == resolution.width && Screen.currentResolution.height == resolution.height)
            {
                currentResolution = options.Count - 1;
            }
        }

        if (currentResolution == -1) Application.Quit();

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
    }

    public void ChangeTab(int index)
    {
        ChangeTab(index, false);
    }

    public void ChangeTab(int index, bool forceUpdate)
    {
        if (!forceUpdate && panels[index].activeInHierarchy) return;

        currentTabIndex = index;

        if (currentTabIndex == 0) ReloadGeneralMenu();

        if (currentTabIndex == 1) ReloadKeybindsMenu();
        else ApplyKeybindChanges();

        for (int i = 0; i < panels.Count; i++)
        {
            panels[i].SetActive(i == index);
            tabs[i].GetComponent<Image>().color = (i == index) ? new Color32(49, 54, 56, 255) : new Color32(236, 100, 97, 255);
        }
    }

    public void ChangeTab(GameObject panel)
    {
        ChangeTab(panels.IndexOf(panel));
    }

    public void SetResolution(int index)
    {
        Resolution res = resolutions[index];
        Screen.SetResolution(res.width, res.height, Screen.fullScreenMode);
    }

    public void SetWindowMode(int index)
    {
        Screen.fullScreenMode = windowModes[index];
    }
}
