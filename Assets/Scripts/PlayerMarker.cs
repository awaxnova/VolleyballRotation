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
    // Start is called before the first frame update
    void Start()
    {
        // Find the rotation manager
        rotationManager = GameObject.Find("Managers").GetComponent<RotationManager>();
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
            Debug.Log($"New position: {transform.position}");
        }
    }
}
