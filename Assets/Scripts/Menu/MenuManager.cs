using System.Collections.Generic;
using UnityEngine;

public class MenuManager : MonoBehaviour
{
    public static MenuManager instance;

    [SerializeField] private List<Menu> menuList; //Serialize field, fill all menus here.
    private Dictionary<string, Menu> menus; //This dictionary is used for better search.

    [SerializeField] private GameObject background;

    private Menu current; //Menu that is currently opened.

    void Awake()
    {
        instance = this;
        menus = new Dictionary<string, Menu>();
    }

    void Start()
    {
        foreach (var item in menuList)
        {
            menus.Add(item.GetName(), item);
        }
    }

    void Update() {}

    public bool IsOpened(string uiName)
    {
        if (!menus.TryGetValue(uiName, out Menu menu)) return false;
        return menu.IsOpened();
    }

    /// <summary>
    /// Toggles UI based on the name.
    /// </summary>
    /// <param name="uiName">The UI name</param>
    public bool ToggleUI(string uiName) => ToggleUI(uiName, false);

    /// <summary>
    /// Toggles UI based on the name.
    /// </summary>
    /// <param name="uiName">The UI name</param>
    /// <param name="closeAll">If true all active menus will close before open</param>
    public bool ToggleUI(string uiName, bool closeAll)
    {
        if (!closeAll && current != null && !current.IsCloseable()) return false; //If the current menu is not closeable then return.

        if (!menus.TryGetValue(uiName, out Menu menu))
        {
            Debug.LogWarning($"Cannot find UI {uiName} that you are trying to toggle.");
            return false;
        }

        if(closeAll) CloseAll();
        menu.ToggleUI();

        bool isOpened = menu.IsOpened();
        if(background != null) background.SetActive(isOpened);

        return isOpened;
;    }

    /// <summary>
    /// Opens menu and closes all curent active ones.
    /// </summary>
    /// <param name="uiName">The UI name</param>
    public void Open(string uiName)
    {
        if (!menus.TryGetValue(uiName, out Menu menu))
        {
            Debug.LogWarning($"Cannot find UI {uiName} that you are trying to open.");
            return;
        }

        CloseAll();
        menu.Open();
        if (background != null) background.SetActive(true);
    }

    /// <summary>
    /// Closes selected UI.
    /// </summary>
    /// <param name="menu">The menu</param>
    public void Close(Menu menu)
    {
        menu.Close();
        if (background != null) background.SetActive(false);
    }


    /// <summary>
    /// Closes selected UI. (even uncloseable ones)
    /// </summary>
    /// <param name="menu">The ui name</param>
    public void Close(string uiName)
    {
        if (!menus.TryGetValue(uiName, out Menu menu))
        {
            Debug.LogWarning($"Cannot find UI {uiName} that you are trying to close.");
            return;
        }

        Close(menu);
    }

    /// <summary>
    /// Closes all menus. (even uncloseable ones)
    /// </summary>
    public void CloseAll()
    {
        foreach (Menu menu in menus.Values)
        {
            Close(menu);
        }
    }

    /// <summary>
    /// Closes currently opened UI, however cannot close uncloseable menus.
    /// </summary>
    /// <returns>true if any menu was closed, otherwise false</returns>
    public bool CloseCurrent()
    {
        if (current == null || !current.IsCloseable()) return false;
        ToggleUI(current.GetName());
        return true;
    }

    public bool IsAnyOpened() => current != null;
    public void SetCurrentMenu(Menu menu) => current = menu;
}
