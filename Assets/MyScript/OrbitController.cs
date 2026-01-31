using UnityEngine;

public class OrbitController : MonoBehaviour
{
    [Header("References")]
    public GameObject Moon;
    public GameObject Planet;
    public PlanetInteractionController InteractionController;

    [Header("Orbit Settings")]
    public float OrbitSpeed = 30f;
    public float OrbitRadius = 1.5f;
    public float MoonScale = 0.27f; // Moon is about 27% of Earth's size

    [Header("Planet Rotation (in Orbit Mode)")]
    public Vector3 PlanetRotationSpeed = new Vector3(0, 20, 0);

    [Header("State")]
    public bool IsOrbitMode = false;

    private float currentAngle = 0f;
    private Vector3 originalMoonScale;
    private bool wasAutoRotateEnabled;

    void Start()
    {
        // Hide moon initially
        if (Moon != null)
        {
            originalMoonScale = Moon.transform.localScale;
            Moon.SetActive(false);
        }
    }

    void Update()
    {
        if (IsOrbitMode && Moon != null && Planet != null)
        {
            // Auto-rotate the planet
            Planet.transform.Rotate(PlanetRotationSpeed * Time.deltaTime);

            // Update orbit angle
            currentAngle += OrbitSpeed * Time.deltaTime;
            if (currentAngle >= 360f) currentAngle -= 360f;

            // Calculate moon position around planet
            float x = Planet.transform.position.x + Mathf.Cos(currentAngle * Mathf.Deg2Rad) * OrbitRadius;
            float z = Planet.transform.position.z + Mathf.Sin(currentAngle * Mathf.Deg2Rad) * OrbitRadius;
            float y = Planet.transform.position.y;

            Moon.transform.position = new Vector3(x, y, z);

            // Make moon face the planet (optional)
            Moon.transform.LookAt(Planet.transform);
        }
    }

    public void ToggleOrbitMode()
    {
        IsOrbitMode = !IsOrbitMode;

        if (IsOrbitMode)
        {
            EnterOrbitMode();
        }
        else
        {
            ExitOrbitMode();
        }
    }

    void EnterOrbitMode()
    {
        // Disable interaction controller
        if (InteractionController != null)
        {
            wasAutoRotateEnabled = InteractionController.AutoRotate;
            InteractionController.enabled = false;
        }

        // Show and position moon
        if (Moon != null && Planet != null)
        {
            Moon.SetActive(true);

            // Set moon scale relative to planet
            float planetScale = Planet.transform.localScale.x;
            Moon.transform.localScale = Vector3.one * planetScale * MoonScale;

            // Set orbit radius based on planet scale
            OrbitRadius = planetScale * 1.5f;

            // Position moon at starting point
            currentAngle = 0f;
            Vector3 startPos = Planet.transform.position + Vector3.right * OrbitRadius;
            Moon.transform.position = startPos;
        }
    }

    void ExitOrbitMode()
    {
        // Re-enable interaction controller
        if (InteractionController != null)
        {
            InteractionController.enabled = true;
            InteractionController.AutoRotate = wasAutoRotateEnabled;
        }

        // Hide moon
        if (Moon != null)
        {
            Moon.SetActive(false);
        }
    }

    // Call this if planet scale changes (for future use)
    public void UpdateOrbitRadius()
    {
        if (Planet != null)
        {
            OrbitRadius = Planet.transform.localScale.x * 1.5f;

            if (Moon != null)
            {
                float planetScale = Planet.transform.localScale.x;
                Moon.transform.localScale = Vector3.one * planetScale * MoonScale;
            }
        }
    }
}
