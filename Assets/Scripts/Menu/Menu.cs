using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class Menu : MonoBehaviour
{

    [SerializeField] private UnityEvent onOpen; //Action that will be called on open.

    [SerializeField] private List<GameObject> tabs;
    [SerializeField] private List<GameObject> pages;

    [SerializeField] private GameObject panel;
    [SerializeField] private Button closeButton;

    void Start() {
        if (closeButton == null) return;
        closeButton.interactable = true;
        closeButton.onClick.RemoveAllListeners();
        closeButton.onClick.AddListener(() => MenuManager.instance.ToggleUI(GetName()));
    }

    void Update() { }

    public void ToggleUI()
    {
        if(IsOpened())
        {
            panel.SetActive(false);
            return;
        }

        onOpen?.Invoke();
        panel.SetActive(true);
        if(tabs.Count > 0) ChangeTab(0); //Always open home tab on first open.
    }

    public void Close() => panel.SetActive(false);
    public bool IsOpened() => panel.activeSelf;
    

    public void ChangeTab(int index)
    {
        int active = GetActiveIndex();
        if (active == index) return;

        for (int i = 0; i < pages.Count; i++)
        {
            pages[i].SetActive(i == index);

            CustomButton customButton = tabs[i].GetComponent<CustomButton>();
            customButton.ChangeColorGroup(i == index ? ButtonColor.Pink : ButtonColor.Gray);
            customButton.ToggleDisabled(i == index);
        }
    }

    private int GetActiveIndex() => pages.FindIndex(page => page.activeSelf);

    public string GetName() => gameObject.name;
}
