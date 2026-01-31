using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class PositionInFrontOfCamera : MonoBehaviour
{
    [Header("Settings")]
    public float DistanceFromCamera = 2f;
    public float HeightOffset = 0f; // Adjust if you want planet higher/lower

    [Header("Status")]
    public bool IsPositioned = false;

    private Camera arCamera;
    private ARSession arSession;
    private float waitTime = 0f;
    private float maxWaitTime = 2f; // Wait max 2 seconds for AR to be ready

    void Start()
    {
        arCamera = Camera.main;
        arSession = FindAnyObjectByType<ARSession>();
        IsPositioned = false;
    }

    void Update()
    {
        // Only position once
        if (IsPositioned) return;

        // Wait a moment for AR tracking to initialize
        waitTime += Time.deltaTime;

        // Check if AR is ready OR we've waited long enough
        bool arReady = (arSession != null && ARSession.state == ARSessionState.SessionTracking);
        bool waitedEnough = (waitTime >= maxWaitTime);

        if (arReady || waitedEnough)
        {
            PositionObjectInFrontOfCamera();
            IsPositioned = true;
        }
    }

    void PositionObjectInFrontOfCamera()
    {
        if (arCamera == null)
        {
            arCamera = Camera.main;
            if (arCamera == null) return;
        }

        // Get camera position and forward direction
        Vector3 cameraPos = arCamera.transform.position;
        Vector3 cameraForward = arCamera.transform.forward;

        // Calculate position in front of camera
        // We only use the horizontal forward direction (ignore camera tilt)
        Vector3 horizontalForward = new Vector3(cameraForward.x, 0, cameraForward.z).normalized;

        // If looking straight up/down, use camera's general forward
        if (horizontalForward.magnitude < 0.1f)
        {
            horizontalForward = cameraForward;
        }

        // Final position: in front of camera at specified distance
        Vector3 newPosition = cameraPos + horizontalForward * DistanceFromCamera;

        // Apply height offset (planet stays at camera height + offset)
        newPosition.y = cameraPos.y + HeightOffset;

        transform.position = newPosition;

        Debug.Log("Planet positioned at: " + newPosition);
    }

    // Call this from a UI button to re-center the planet in front of you
    public void RecenterPlanet()
    {
        IsPositioned = false;
        waitTime = maxWaitTime; // Skip waiting, position immediately
    }
}
