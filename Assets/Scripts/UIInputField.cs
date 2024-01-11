using DrawXXL;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

public class UIInputField : MonoBehaviour
{
    public InputField inputField;

    private RectTransform rectTransform;

    private RectTransform canvasRect;

    private Vector2 originalAnchorMin;
    private Vector2 originalAnchorMax;
    private Vector2 originalPivot;
    private Vector2 originalAnchoredPosition;

    private bool lastKeyboardVisible = false;

    private Transform originalParent;
    public Canvas temporaryCanvas; // Reference to a Canvas with a higher sorting order

    public Toggle enableTextInputFieldAboveKeyboard;

    public float margin = 1f;


    private void Start()
    {
        // Get the input field component
        inputField = GetComponent<InputField>();

        // Get the RectTransform of the InputField
        rectTransform = inputField.GetComponent<RectTransform>();

        canvasRect = GetComponentInParent<Canvas>().GetComponent<RectTransform>();
        // Save the original anchor values when the scene starts
        SaveOriginalAnchor();
    }

    private void Update()
    {
        // Check if the TouchScreenKeyboard is visible
        if (!lastKeyboardVisible && TouchScreenKeyboard.visible) // Just became visible
        {
            // Move the InputField above the keyboard
            AnchorToTopCenter();
        }
        else if(lastKeyboardVisible && !TouchScreenKeyboard.visible)
        {
            // Reset InputField to its original position when keyboard is closed
            RevertAnchor();           
        }

        UpdateMoveOnScreen();

        lastKeyboardVisible = TouchScreenKeyboard.visible;
    }

    [Button("UpdateMoveOnScreen")]
    void UpdateMoveOnScreen() 
    {

        // Get the screen boundaries of the InputField
        Vector3[] corners = new Vector3[4];
        rectTransform.GetWorldCorners(corners);

        // Check if InputField is off-screen
        bool isOffScreen = false;
        Rect screenBounds = new Rect(0f, 0f, Screen.width, Screen.height);

        foreach (Vector3 corner in corners)
        {
            if (!screenBounds.Contains(corner))
            {
                isOffScreen = true;
                break;
            }
        }

        // If off-screen, move it on-screen with a margin using anchoredPosition
        if (isOffScreen)
        {
            Vector2 newPosition = rectTransform.anchoredPosition;

            // Calculate movement needed to bring it on-screen with a margin
            if (corners[0].x < 0) // Left edge
            {
                newPosition.x -= corners[0].x - margin;
            }
            else if (corners[2].x > Screen.width) // Right edge
            {
                newPosition.x -= corners[2].x - Screen.width + margin;
            }

            if (corners[0].y < 0) // Bottom edge
            {
                newPosition.y -= corners[0].y - margin;
            }
            else if (corners[1].y > Screen.height) // Top edge
            {
                newPosition.y -= corners[1].y - Screen.height + margin;
            }

            // Apply the new anchoredPosition to the InputField
            rectTransform.anchoredPosition = newPosition;
        }
    }

    [Button("SaveOriginalAnchor")]
    void SaveOriginalAnchor()
    {
        // Store the original anchor values
        originalAnchorMin = rectTransform.anchorMin;
        originalAnchorMax = rectTransform.anchorMax;
        originalPivot = rectTransform.pivot;

        originalAnchoredPosition = rectTransform.anchoredPosition;

        originalParent = inputField.transform.parent;
    }

    [Button("AnchorToTopCenter")]
    public void AnchorToTopCenter()
    {
        // Get the canvas dimensions
        Canvas canvas = GetComponentInParent<Canvas>();
        Rect canvasRect = canvas.pixelRect;

        // Calculate the position 10% from the top of the canvas in pixels
        //float offsetFromTop = canvasRect.height * 0.65f;
        float offsetFromTop = Screen.height * 0.40f;

        float offsetPercentageFromTop = 0.05f;

        rectTransform.anchorMin = new Vector2(0.5f, 1f - offsetPercentageFromTop);
        rectTransform.anchorMax = new Vector2(0.5f, 1f - offsetPercentageFromTop);
        
        // Set the anchored position to be 10% from the top of the canvas in screen space
        rectTransform.anchoredPosition = rectTransform.anchoredPosition + new Vector2(0,0);

        inputField.transform.SetParent(temporaryCanvas.transform, false);

        TouchScreenKeyboard.hideInput = !enableTextInputFieldAboveKeyboard.isOn;
    }

    [Button("RevertAnchor")]
    public void RevertAnchor()
    {
        // Revert to the original anchor values
        rectTransform.anchorMin = originalAnchorMin;
        rectTransform.anchorMax = originalAnchorMax;
        rectTransform.pivot = originalPivot;

        // Optionally, adjust the anchored position if needed
        rectTransform.anchoredPosition = originalAnchoredPosition;

        inputField.transform.SetParent(originalParent, false);
    }

    [Button("TestDrawPoints")]
    public void TestDrawPoints()
    { 

        // Instantiate a red sphere at the center of the canvas
        GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        sphere.transform.position = new Vector3(0f, 0f, 0f);
        sphere.GetComponent<Renderer>().material.color = Color.red;

        // Instantiate a sphere at the bottom right most corner of the canvas.
        // Instantiate a green sphere at the bottom left most corner of the canvas.
        // Instantiate a black cube at the upper right most corner of the canvas.

        GameObject sphere2 = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        sphere2.transform.position = new Vector3(canvasRect.sizeDelta.x, 0f, 0f);
        sphere2.GetComponent<Renderer>().material.color = Color.green;

        GameObject sphere3 = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        sphere3.transform.position = new Vector3(0f, 0f, 0f);
        sphere3.GetComponent<Renderer>().material.color = Color.black;

        GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        cube.transform.position = new Vector3(canvasRect.sizeDelta.x, canvasRect.sizeDelta.y, 0f);
        cube.GetComponent<Renderer>().material.color = Color.yellow;

    }

    Vector2 ScreenToCanvasSpace(Vector2 screenPosition, RectTransform canvas)
    {
        Vector2 viewportPosition = Camera.main.ScreenToViewportPoint(screenPosition);
        return new Vector2(viewportPosition.x * canvas.sizeDelta.x, viewportPosition.y * canvas.sizeDelta.y);
    }

}

