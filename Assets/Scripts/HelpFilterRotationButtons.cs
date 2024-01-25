
using UnityEngine;
using UnityEngine.UI;
using VolleyballRotation;

public class HelpFilterRotationButtons : HelpFilter
{
    // Start is called before the first frame update

    [Tooltip("The rotation buttons that we want to filter for their positions.")]
    public Button[] rotationButtons;

    private RotationManager rotationManager;

    private static int invocationCount = -1;

    void Start()
    {
        rotationManager = GameObject.Find("Managers").GetComponent<RotationManager>();       
    }
    
    // Update is called once per frame
    void Update()
    {
        
    }

    public override void RestartHelpChain()
    {
        invocationCount = -1;
    }

    /// <summary>
    /// The first time through, 0th index, find one that's not selected...
    /// the next time through, find the nextRotation.
    /// </summary>
    /// <param name="optionIndex"></param>
    /// <returns></returns>
    public override GameObject filterForGameObject()
    {
        int currentRotation = rotationManager.currentRotation;
        int nextRotation = rotationManager.nextRotation;
        
        invocationCount++;

        // Based on which rotation we're in, return one that's not the current or next rotation.
        for (int rotationIdx = 1; rotationIdx <= 6; rotationIdx++)
        {
            if ((invocationCount & 0x1) == 0x1) // odd index
            {
                // Find the first one that's already clicked/selected as nextRotation.
                if (rotationIdx == nextRotation)
                {
                    return rotationButtons[rotationIdx - 1].gameObject;
                }
            }
            else // even index, such as the 0th time through the loop.
            {
                // Find the first one that's not already clicked/selected.
                if (rotationIdx != currentRotation && rotationIdx != nextRotation)
                {
                    // return the first rotation that isn't the currentRotation or the nextRotation.
                    // Value values are 1 through 6.
                    return rotationButtons[rotationIdx - 1].gameObject;
                }

            }
        }

        // by default, return null
        return null;
    }

}
