using System.Collections;
using UnityEngine;

public class IconCamera : MonoBehaviour
{

    void Awake()
    {
        StartCoroutine(RenderCamera());
    }

    void Update() {}

    public IEnumerator RenderCamera()
    {
        Camera cam = GetComponent<Camera>();
        cam.enabled = true;
        cam.Render();
        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();
        cam.gameObject.SetActive(false);
    }
}