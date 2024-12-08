using UnityEngine;

public class CanvasCamerFix : MonoBehaviour
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
