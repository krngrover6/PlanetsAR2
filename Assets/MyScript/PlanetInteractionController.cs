using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.EnhancedTouch;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;

public class PlanetInteractionController : MonoBehaviour
{
    [Header("Target")]
    public GameObject PlanetObject;

    [Header("Rotation Settings")]
    public float RotationSpeed = 0.2f;
    public bool InvertRotation = false;

    [Header("Scale Settings")]
    public float MinScale = 0.1f;
    public float MaxScale = 3.0f;
    public float PinchSensitivity = 0.01f;

    [Header("Auto Rotation")]
    public bool AutoRotate = true;
    public Vector3 AutoRotationSpeed = new Vector3(0, 20, 0);

    private Vector2 previousTouchPosition;
    private float previousPinchDistance;
    private bool isInteracting = false;

    void OnEnable()
    {
        // Enable Enhanced Touch support for the new Input System
        EnhancedTouchSupport.Enable();
    }

    void OnDisable()
    {
        EnhancedTouchSupport.Disable();
    }

    void Update()
    {
        if (PlanetObject == null) return;

        int activeTouches = Touch.activeTouches.Count;

        // Handle auto rotation when not being touched
        if (AutoRotate && activeTouches == 0)
        {
            PlanetObject.transform.Rotate(AutoRotationSpeed * Time.deltaTime);
        }

        // Handle touch input
        HandleTouchInput();
    }

    void HandleTouchInput()
    {
        var touches = Touch.activeTouches;
        int touchCount = touches.Count;

        if (touchCount == 0)
        {
            isInteracting = false;
            return;
        }

        // Check if touching UI
        if (IsTouchOverUI(touches[0].screenPosition))
        {
            return;
        }

        // Single touch - Rotate
        if (touchCount == 1)
        {
            HandleRotation(touches[0]);
        }
        // Two touches - Pinch to scale
        else if (touchCount >= 2)
        {
            HandlePinchScale(touches[0], touches[1]);
        }
    }

    bool IsTouchOverUI(Vector2 position)
    {
        if (EventSystem.current == null) return false;

        // Check if touch is over any UI element
        PointerEventData eventData = new PointerEventData(EventSystem.current);
        eventData.position = position;

        var results = new System.Collections.Generic.List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, results);

        return results.Count > 0;
    }

    void HandleRotation(Touch touch)
    {
        if (touch.phase == UnityEngine.InputSystem.TouchPhase.Began)
        {
            previousTouchPosition = touch.screenPosition;
            isInteracting = true;
        }
        else if (touch.phase == UnityEngine.InputSystem.TouchPhase.Moved && isInteracting)
        {
            Vector2 currentPosition = touch.screenPosition;
            Vector2 delta = currentPosition - previousTouchPosition;

            float rotationX = delta.y * RotationSpeed;
            float rotationY = -delta.x * RotationSpeed;

            if (InvertRotation)
            {
                rotationX = -rotationX;
                rotationY = -rotationY;
            }

            // Rotate around world axes for intuitive control
            PlanetObject.transform.Rotate(Vector3.up, rotationY, Space.World);
            PlanetObject.transform.Rotate(Vector3.right, rotationX, Space.World);

            previousTouchPosition = currentPosition;
        }
        else if (touch.phase == UnityEngine.InputSystem.TouchPhase.Ended ||
                 touch.phase == UnityEngine.InputSystem.TouchPhase.Canceled)
        {
            isInteracting = false;
        }
    }

    void HandlePinchScale(Touch touch0, Touch touch1)
    {
        // Calculate current distance between touches
        float currentPinchDistance = Vector2.Distance(touch0.screenPosition, touch1.screenPosition);

        if (touch0.phase == UnityEngine.InputSystem.TouchPhase.Began ||
            touch1.phase == UnityEngine.InputSystem.TouchPhase.Began)
        {
            previousPinchDistance = currentPinchDistance;
            isInteracting = true;
            return;
        }

        if (touch0.phase == UnityEngine.InputSystem.TouchPhase.Moved ||
            touch1.phase == UnityEngine.InputSystem.TouchPhase.Moved)
        {
            if (isInteracting && previousPinchDistance > 0)
            {
                // Calculate scale change
                float pinchDelta = currentPinchDistance - previousPinchDistance;
                float scaleChange = pinchDelta * PinchSensitivity;

                // Apply scale
                Vector3 newScale = PlanetObject.transform.localScale + Vector3.one * scaleChange;

                // Clamp scale within limits
                float clampedScale = Mathf.Clamp(newScale.x, MinScale, MaxScale);
                PlanetObject.transform.localScale = Vector3.one * clampedScale;

                previousPinchDistance = currentPinchDistance;
            }
        }

        if (touch0.phase == UnityEngine.InputSystem.TouchPhase.Ended ||
            touch1.phase == UnityEngine.InputSystem.TouchPhase.Ended ||
            touch0.phase == UnityEngine.InputSystem.TouchPhase.Canceled ||
            touch1.phase == UnityEngine.InputSystem.TouchPhase.Canceled)
        {
            isInteracting = false;
        }
    }

    #region Public Methods (for UI buttons)

    public void ToggleAutoRotate()
    {
        AutoRotate = !AutoRotate;
    }

    public void SetAutoRotate(bool enabled)
    {
        AutoRotate = enabled;
    }

    public void ResetScale()
    {
        if (PlanetObject != null)
        {
            PlanetObject.transform.localScale = Vector3.one;
        }
    }

    public void ResetRotation()
    {
        if (PlanetObject != null)
        {
            PlanetObject.transform.rotation = Quaternion.identity;
        }
    }

    #endregion
}
