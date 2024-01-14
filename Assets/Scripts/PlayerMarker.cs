using Arrow;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VolleyballRotation;

public class PlayerMarker : MonoBehaviour
{
    [Range(1, 6)]
    public int playerNumber;
    private RotationManager rotationManager;
    private bool isDragging = false;

    private Vector3 startingPosition;  // Store the initial player position on click

    public SuperTextMesh superTextMeshComponent;
    private BoxCollider boxColliderComponent;
    private STMMatchRect boxColliderStmMatchRect;

    private RectTransform boxColliderRT;
    public AnimatedArrowRenderer arrowRenderer;

    // Start is called before the first frame update
    void Start()
    {
        // Find the rotation manager
        rotationManager = GameObject.Find("Managers").GetComponent<RotationManager>();

        // Assuming you have a reference to the SuperTextMesh component and the BoxCollider2D component
        superTextMeshComponent  = GetComponentInChildren<SuperTextMesh>();
        boxColliderComponent = GetComponent<BoxCollider>();
        boxColliderStmMatchRect = boxColliderComponent.GetComponentInChildren<STMMatchRect>();
        boxColliderRT = boxColliderStmMatchRect.GetComponent<RectTransform>();
        arrowRenderer = GetComponentInChildren<AnimatedArrowRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if (isDragging)
        {
            OnDrag();
        }
    }

    private void OnMouseDown()
    {
        Debug.Log($"Mouse down ({playerNumber})");
        startingPosition = transform.position;
        isDragging = true;
    }

    private void OnMouseUp()
    {
        Debug.Log($"Mouse up ({playerNumber})");
        isDragging = false;

        if(rotationManager.ValidatePositions())
        {
            // Update the player's position in the rotation manager
            rotationManager.SavePlayerPosition(this);
        }
        else
        {
            // Revert the player's position if it's invalid.
            transform.position = startingPosition;
        }
    }

    // When the player marker is dragged, update the player's position, only in the x and z dimensions
    private void OnDrag()
    {
        // Create a ray from the screen point
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        // Declare a variable to store the hit information
        RaycastHit hit;

        // Perform the raycast
        if (Physics.Raycast(ray, out hit))
        {
            // Set the object's position to the hit point
            // Maintain the Up position
            transform.position = new Vector3(hit.point.x, transform.position.y, hit.point.z);

            // Debug print the new position
            //Debug.Log($"New position: {transform.position}");
        }
    }
    public void UpdateBoxColliderToMatchSuperTextMesh()
    {
        if (superTextMeshComponent != null && boxColliderComponent != null)
        {
            // Update the BoxCollider2D size to match the SuperTextMesh size
            // The BoxCollider should match the Rect Transform of this GameObject
            boxColliderStmMatchRect.Match();
            boxColliderComponent.size =  new Vector3( boxColliderRT.sizeDelta.x, boxColliderRT.sizeDelta.y, boxColliderComponent.size.z);
        }
    }
}
