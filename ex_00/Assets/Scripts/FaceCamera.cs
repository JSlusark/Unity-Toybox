using UnityEngine;

public class FaceCamera : MonoBehaviour
{
    void LateUpdate()
    {
        if (Camera.main == null) return;

        // Make the quad face the camera
        transform.LookAt(Camera.main.transform);

        // Quads in Unity face backwards, so flip 180 degrees
        transform.Rotate(0, 10f, 0);
    }
}
