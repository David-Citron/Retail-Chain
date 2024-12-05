using UnityEngine;

public class FaceCamera : MonoBehaviour
{

    private void Start()
    {
        
    }
    void Update()
    {
        // Ensure the main camera exists
        if (Camera.main != null)
        {
            // Get the direction to the camera
            Vector3 directionToCamera = Camera.main.transform.position - transform.position;

            // Calculate the rotation to face the camera
            Quaternion targetRotation = Quaternion.LookRotation(directionToCamera);

            // Apply the rotation to the GameObject
            transform.rotation = targetRotation;
        }
    }
}