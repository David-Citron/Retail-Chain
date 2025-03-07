using System.Collections.Generic;
using UnityEngine;

public class MenuManager : MonoBehaviour
{
    public static MenuManager instance;

    [SerializeField] private List<Menu> menuList; //Serialize field, fill all menus here.
    private Dictionary<string, Menu> menus; //This dictionary is used for better search.

    [SerializeField] private GameObject background;

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
        if (!menus.TryGetValue(uiName, out Menu menu))
        {
            Debug.LogWarning("Cannot find UI that you are trying to open.");
            return false;
        }

        return menu.IsOpened();
    }

    /// <summary>
    /// Toggles UI based on the name and closes all active menus before open.
    /// </summary>
    /// <param name="uiName">The UI name</param>
    public void ToggleUI(string uiName) => ToggleUI(uiName, true);

    /// <summary>
    /// Toggles UI based on the name.
    /// </summary>
    /// <param name="uiName">The UI name</param>
    public void ToggleUI(string uiName, bool closeAll)
    {
        if(!menus.TryGetValue(uiName, out Menu menu)) {
            Debug.LogWarning("Cannot find UI that you are trying to open.");
            return;
        }

        if(closeAll) CloseAll();
        menu.ToggleUI();

        bool isOpened = menu.IsOpened();
        if(background != null) background.SetActive(isOpened);
;    }

    /// <summary>
    /// Opens menu and closes all curent active ones.
    /// </summary>
    /// <param name="uiName">The UI name</param>
    public void Open(string uiName)
    {
        if (!menus.TryGetValue(uiName, out Menu menu))
        {
            Debug.LogWarning("Cannot find UI that you are trying to open.");
            return;
        }

        CloseAll();
        menu.Open();
    }

    public void Close(string uiName)
    {
        if (!menus.TryGetValue(uiName, out Menu menu))
        {
            Debug.LogWarning("Cannot find UI that you are trying to open.");
            return;
        }

        menu.Close();
    }

    public void CloseAll()
    {
        foreach (Menu menu in menus.Values)
        {
            menu.Close();
        }
    }
}
