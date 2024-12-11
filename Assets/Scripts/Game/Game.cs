using Mirror;
using UnityEngine;

public class Game : MonoBehaviour
{

    private static Game instance;
    private GameState gameState;

    void Start()
    {
        instance = this;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void setGameState(GameState newState)
    {
        if (gameState == newState) return;

        gameState = newState;

        switch(gameState)
        {
            case GameState.Ending:
                NetworkManager.singleton.ServerChangeScene("MainMenu");
                break;
        }
    }

    public static Optional<Game> Instance() => Optional<Game>.Of(instance);
}
