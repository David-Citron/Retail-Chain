using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public Scene scene;

    private void Start()
    {
        Instance = this;

        if (SceneManager.GetActiveScene().buildIndex != 0) return;
        LayoutManager.instance.InicializeHostButton();
    }

    public void SetScene(Scene newScene)
    {
        this.scene = newScene;
    }
}
