using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class LayoutManager : MonoBehaviour
{

    [SerializeField]
    private GameObject defaultButtonsGroup;

    [SerializeField]
    private GameObject playButtonsGroup;

    [SerializeField]
    private GameObject lobby;


    void Start()
    {
        defaultButtonsGroup.SetActive(true);
        playButtonsGroup.SetActive(false);
        lobby.SetActive(false);
    }

    public void ChangeActive(GameObject button)
    {
        button.SetActive(!button.activeInHierarchy);
    }

    public void BackToMainMenu(GameObject current)
    {
        defaultButtonsGroup.SetActive(true);
        current.SetActive(false);
    }

    public void ExitGame()
    {
        Application.Quit();
    }

}
