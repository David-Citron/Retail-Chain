using System.Collections.Generic;
using UnityEngine;

public abstract class MenuManager : MonoBehaviour
{

    public static MenuManager instance;

    [SerializeField] private List<GameObject> disabledItems; //UI elements that should be disabled during some UI is open.
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

    public void ToggleUI(string uiName)
    {
        if(!menus.TryGetValue(uiName, out Menu menu)) {
            Debug.LogWarning("Cannot find UI that you are trying to open.");
            return;
        }

        menu.ToggleUI();

        bool isOpened = menu.IsOpened();
        background?.SetActive(isOpened);
        disabledItems?.ForEach(item => item.SetActive(!isOpened));
;    }

    public void CloseAll()
    {
        foreach (Menu menu in menus.Values)
        {
            menu.Close();
        }
    }
}
