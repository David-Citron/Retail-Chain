using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Help : MonoBehaviour
{

    [SerializeField] private List<GameObject> tabs;
    [SerializeField] private List<GameObject> pages;

    [SerializeField] private Button closeButton;


    void Start() {}
    void Update() {}


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
}
