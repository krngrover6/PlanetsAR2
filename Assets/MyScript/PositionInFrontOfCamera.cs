using UnityEngine;

public class PositionInFrontOfCamera : MonoBehaviour
{
    [Header("Settings")]
    public float DistanceFromCamera = 2f;
    public bool PositionOnStart = true;
    public bool FollowCamera = false;

    private Camera arCamera;

    void Start()
    {
        // Find the AR camera
        arCamera = Camera.main;

        if (PositionOnStart && arCamera != null)
        {
            PositionObjectInFrontOfCamera();
        }
    }

    void LateUpdate()
    {
        // If follow mode is enabled, keep the object in front of camera
        if (FollowCamera && arCamera != null)
        {
            PositionObjectInFrontOfCamera();
        }
    }

    public void PositionObjectInFrontOfCamera()
    {
        if (arCamera == null)
        {
            arCamera = Camera.main;
            if (arCamera == null) return;
        }

        // Position the object in front of the camera
        Vector3 cameraForward = arCamera.transform.forward;
        Vector3 newPosition = arCamera.transform.position + cameraForward * DistanceFromCamera;

        transform.position = newPosition;
    }

    // Call this from a button if you want to re-center the planet
    public void RecenterPlanet()
    {
        PositionObjectInFrontOfCamera();
    }
}
