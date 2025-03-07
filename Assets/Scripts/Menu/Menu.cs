using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class Menu : MonoBehaviour
{

    [SerializeField] private UnityEvent onOpen; //Action that will be called on open.
    [SerializeField] private UnityEvent onClose; //Action that will be called on open.

    [SerializeField] private List<GameObject> tabs;
    [SerializeField] private List<GameObject> pages;
    [SerializeField] private List<GameObject> menuDisabledItems; //These gameobjects are disabled when a menu opens and reactivated when it closes.

    [SerializeField] private Button closeButton;
    [SerializeField] private bool closeable; //If the menu can be closed with "ESC"

    void Start() {
        if (closeButton == null) return;
        closeButton.interactable = true;
        closeButton.onClick.RemoveAllListeners();
        closeButton.onClick.AddListener(() => MenuManager.instance.ToggleUI(GetName(), false));
    }

    void Update() { }

    public void ToggleUI()
    {
        if(IsOpened())
        {
            Close();
            return;
        }

        Open();
    }

    public void Open()
    {
        MenuManager.instance.SetCurrentMenu(this);
        menuDisabledItems.ForEach(item => item.SetActive(false));
        onOpen?.Invoke();
        gameObject.SetActive(true);
        if (closeable && tabs.Count > 0) ChangeTab(0, true); //Always open home tab on first open when the menu is closeable.
    }

    public void Close()
    {
        MenuManager.instance.SetCurrentMenu(null);
        onClose?.Invoke();
        menuDisabledItems.ForEach(item => item.SetActive(true));
        gameObject.SetActive(false);
    }

    public bool IsOpened() => gameObject.activeSelf;
    
    public void ChangeTab(GameObject page) => ChangeTab(pages.IndexOf(page));
    public void ChangeTab(int index) => ChangeTab(index, false);
    public void ChangeTab(int index, bool force)
    {
        int active = GetActiveIndex();
        if (!force && active == index) return;

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
    public bool IsCloseable() => closeable;
}
