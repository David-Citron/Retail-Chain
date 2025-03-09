using UnityEngine;

public class FaceCamera : MonoBehaviour
{
    public bool swap = false;

    void Update()
    {
        if (Camera.main != null)
        {
            // Get the direction to the camera
            Vector3 directionToCamera = Camera.main.transform.position - transform.position;

            // Flip the direction if swap is true
            if (swap)
            {
                directionToCamera *= -1;
            }

            // Calculate and apply the rotation
            transform.rotation = Quaternion.LookRotation(directionToCamera);
        }
    }
}
