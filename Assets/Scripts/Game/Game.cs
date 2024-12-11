using Mirror;
using UnityEngine;

public class Game : MonoBehaviour
{

    private static Game instance;

    void Start()
    {
        instance = this;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public static Optional<Game> Instance() => Optional<Game>.Of(instance);
}
