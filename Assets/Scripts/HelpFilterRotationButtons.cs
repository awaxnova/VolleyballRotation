using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UI;
using VolleyballRotation;

public class HelpFilterRotationButtons : HelpFilter
{
    // Start is called before the first frame update

    [Tooltip("The rotation buttons that we want to filter for their positions.")]
    public Button[] rotationButtons;

    private RotationManager rotationManager;

    void Start()
    {
        rotationManager = GameObject.Find("Managers").GetComponent<RotationManager>();       
    }
    
    // Update is called once per frame
    void Update()
    {
        
    }

    public override GameObject filterForGameObject()
    {
        int currentRotation = rotationManager.currentRotation;
        int nextRotation = rotationManager.nextRotation;

        // Based on which rotation we're in, return one that's not the current or next rotation.
        for (int rotationIdx = 1; rotationIdx <= 6; rotationIdx++)
        {
            if (rotationIdx != currentRotation && rotationIdx != nextRotation)
            {
                // return the first rotation that isn't the currentRotation or the nextRotation.
                // Value values are 1 through 6.
                return rotationButtons[rotationIdx - 1].gameObject;
            }
        }

        // by default, return null
        return null;
    }

}
