using UnityEngine;

public class CanvasCameraFix : MonoBehaviour
{

    private void Awake()
    {
        Canvas canvas = GetComponent<Canvas>();
        canvas.worldCamera = Camera.main;
    }

    void Start()
    {
    }

    void Update()
    {
        
    }
}
