using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Put this on a GameObject that wants to be enabled/disabled when the help is enabled/disabled.
/// </summary>
public class HelpListener : MonoBehaviour
{
    public HelpType helpType;

    // Start is called before the first frame update
    void Start()
    {
        // Check the enable state of the help channel, and enable/disable the GameObject
        gameObject.SetActive(HelpManager.Instance.GetHelpType(helpType));

        // Subscribe to the HelpManager's delegate to be notified when the help is enabled or disabled,
        // and track the enable state of the help channel, by enabling/disabling the GameObject.
        HelpManager.Instance.SetHelpCallback(helpType, HelpUpdated);

        //// Find all GameObjects with a HelpListener component, and subscribe to the HelpUpdated delegate
        //HelpListener[] helpListeners = FindObjectsOfType<HelpListener>();
        //foreach (HelpListener helpListener in helpListeners)
        //{
        //    SetHelpCallback(helpListener.helpType, helpListener.HelpUpdated);
        //}
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /// <summary>
    /// Subscribe to the HelpManager's delegate to be notified when the help is enabled or disabled,
    /// and track the enable state of the help channel, by enabling/disabling the GameObject.
    /// </summary>
    /// <param name="helpType"></param>
    /// <param name="enable"></param>
    public void HelpUpdated(HelpType helpType, bool enable)
    {
        if (helpType == this.helpType)
        {
            Debug.Log($"HelpUpdated: {helpType} {enable}");
            gameObject.SetActive(enable);
        }
    }
}
