using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

/// <summary>
/// Put this on a GameObject that wants to be enabled/disabled when the help is enabled/disabled.
/// </summary>
public class HelpListener : MonoBehaviour
{
    public HelpType helpType;

    [Header("Set this to the next HelpListener in the chain, or leave it null if this is the last in the chain.")]
    public HelpListener nextHelpListener;

    [Header("On the last HelpListener in the chain, set this to the first HelpListener in the chain, or leave it null if we don't want to allow the help chain to restart.")]
    public HelpListener beginningOfChainToReArm;

    public HelpFilter positionFilter;

    public Button[] activators;
    public bool startActive = false;
    public bool startArmed = false;
    public Material buttonIndicatorMaterial;
    private Material[] buttonMaterials;
    private bool isArmed = false;

    // Start is called before the first frame update
    void Start()
    {
        // Check the enable state of the help channel, and enable/disable the GameObject
        gameObject.SetActive(HelpManager.Instance.GetHelpType(helpType));

        // If this help item should start active, then activate it
        gameObject.SetActive(startActive);

        if (startArmed)
            ReArm();

        // populate the buttonMaterials array

        buttonMaterials = new Material[activators.Length];

        for(int activatorIdx= 0; activatorIdx < activators.Length; activatorIdx++)
        {
            Button activator = activators[activatorIdx];
            // Save the original material from the Image component attached to the Button.
            // This is so we can restore it later.

            // Get the Image component attached to the Button
            Image image = activator.GetComponent<Image>();
            // Get the material from the Image component
            Material material = image.material;
            // Save the material
            buttonMaterials[activatorIdx] = material;
        }
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
            Debug.Log($"HelpUpdated: {helpType} {enable} Armed: {isArmed}");
            if(isArmed)
            {
                ActivateAsNext();
                isArmed = false;
            }
        }
    }

    public void ActivateAsNext()
    {
       
        // Activate all children
        Transform[] children = GetComponentsInChildren<Transform>(true);
        foreach (Transform child in children)
        {
            child.gameObject.SetActive(true);
        }

        for(int activatorIndex = 0; activatorIndex < activators.Length; activatorIndex++)
        {
            Button activator = activators[activatorIndex];
            activator.onClick.AddListener(OnActivated);
            SetMaterial(activatorIndex);
        }

        // If we have a position filter, then set the position of this help item to the position of the filter
        if (positionFilter != null)
        {
            transform.position = positionFilter.GetPosition();
        }
    }

    private void SetMaterial(int activatorIndex)
    {
        Button activator = activators[activatorIndex];
        // Get the Image component attached to the Button
        Image image = activator.GetComponent<Image>();

        image.material = buttonIndicatorMaterial;
    }

    public void Deactivate()
    {

        for(int activatorIndex = 0; activatorIndex < activators.Length; activatorIndex++)            
        {
            Button activator = activators[activatorIndex];
            activator.onClick.RemoveListener(OnActivated);
            RestoreMaterial(activatorIndex);
        }
        // Deactivate all children
        Transform[] children = GetComponentsInChildren<Transform>(true);
        foreach (Transform child in children)
        {
            child.gameObject.SetActive(false);
        }
    }

    private void RestoreMaterial(int activatorIndex)
    {
        Button activator = activators[activatorIndex];
        Image image = activator.GetComponent<Image>();
        image.material = buttonMaterials[activatorIndex];
    }

    /// <summary>
    /// This should be called by a UI element when it is activated, and will inform this Help item that it has been activated, and can proceed to the next in the chain, or turn off.
    /// </summary>
    public void OnActivated()
    {
        Debug.Log($"OnActivated: {helpType}");
        Deactivate();

        if (nextHelpListener != null)
            nextHelpListener?.ActivateAsNext();
        else
            ReArmBeginningOfChain();
    }

    /// <summary>
    /// Allow this to listen for the Help button event again.
    /// </summary>
    public void ReArm()
    {
          isArmed = true;
    }

    private void ReArmBeginningOfChain()
    {
        beginningOfChainToReArm?.ReArm();
    }
}
