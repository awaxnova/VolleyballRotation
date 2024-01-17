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
        int currentSituation = (int) rotationManager.currentSituation;
        int nextSituation = (int) rotationManager.nextSituation;

        // Based on which situation we're in, return one that's not the current or next situation.
        for(int i = 0; i < situationButtons.Length; i++)
        {
            if(currentSituation != i && nextSituation != i)
            {
                return situationButtons[i].gameObject;
            }
        }

        // by default, return null
        return null;
    }

}
