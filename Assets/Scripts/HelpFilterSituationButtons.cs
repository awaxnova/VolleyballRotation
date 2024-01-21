using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UI;
using VolleyballRotation;

public class HelpFilterSituationButtons : HelpFilter
{
    // Start is called before the first frame update

    [Tooltip("The situation buttons that we want to filter for their positions.  These should be in the enum declaration order from 0 on up...")]
    public Button[] situationButtons;

    private RotationManager rotationManager;

    private static int invocationCount = -1;

    void Start()
    {
        rotationManager = GameObject.Find("Managers").GetComponent<RotationManager>();
        invocationCount = -1;
    }

    // Update is called once per frame
    void Update()
    {

    }
    public override void RestartHelpChain()
    {
        invocationCount = -1;
    }

    public override GameObject filterForGameObject()
    {
        int currentSituation = (int) rotationManager.currentSituation;
        int nextSituation = (int) rotationManager.nextSituation;

        invocationCount++;

        // Based on which situation we're in, return one that's not the current or next situation.
        for(int i = 0; i < situationButtons.Length; i++)
        {
            if( (invocationCount & 0x1) == 0x1)
            {
                // odd index
                // Find the first one that's already clicked/selected as nextSituation.
                if(i == nextSituation)
                {
                    return situationButtons[i].gameObject;
                }
            }
            else
            {
                // even index, such as the 0th time through the loop.
                // Find the first one that's not already clicked/selected.
                if(i != currentSituation && i != nextSituation)
                {
                    // return the first situation that isn't the currentSituation or the nextSituation.
                    // Value values are 1 through 6.
                    return situationButtons[i].gameObject;
                }
            }
        }

        // by default, return null
        return null;
    }

}
