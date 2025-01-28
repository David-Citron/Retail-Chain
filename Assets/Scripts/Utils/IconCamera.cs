using UnityEngine;

public class IconCamera : MonoBehaviour
{
    // Start is called before the first frame update
    void Awake()
    {
        Camera cam = GetComponent<Camera>();
        cam.enabled = true;
        cam.Render();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
