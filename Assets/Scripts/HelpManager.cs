using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Track the help settings for the user.  This is a singleton.
/// Other scripts can subscribe to the helpEnabledDelegates to be notified when the help is enabled or disabled.
/// </summary>
public class HelpManager : MonoBehaviour
{
    // Create a static instance of the help manager
    public static HelpManager Instance;

    //public bool[] helpEnabled = new bool[3];
    private bool[] helpEnabled = new bool[ /*Number of HelpType*/(int)HelpType.Last];

    // Define a delegate that can be notified of changes in the Help status/enablement.    
    public delegate void HelpUpdatedDelegate(HelpType helpType, bool enable);
    // Create an array of Delegates to invoke
    private HelpUpdatedDelegate[] helpUpdatedDelegates = new HelpUpdatedDelegate[(int)HelpType.Last];

    private UIPlayerSettings uiPlayerSettings;


    private void Awake()
    {
        // If the instance is not null, and it's not this instance, destroy this instance
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            // Assign the static instance
            Instance = this;
            // Keep this object alive between scenes
            DontDestroyOnLoad(this.gameObject);
        }
    }

    
    // Start is called before the first frame update
    void Start()
    {
        // Set all help to disabled
        for (int i = 0; i < helpEnabled.Length; i++)
        {
            helpEnabled[i] = false;
        }

        uiPlayerSettings = FindObjectOfType<UIPlayerSettings>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetHelpType(HelpType helpType, bool enable)
    {

        helpEnabled[(int)helpType] = enable;

        // call the delegate
        helpUpdatedDelegates[(int)helpType]?.Invoke(helpType, enable);
    }

    public bool GetHelpType(HelpType helpType)
    {
        return helpEnabled[(int)helpType];
    }

    public void ClearHelpTypes()
    {
        for (int i = 0; i < helpEnabled.Length; i++)
        {
            SetHelpType((HelpType)i, false);
        }
    }

    public void ToggleHelpType(HelpType helpType)
    {
        SetHelpType(helpType, !helpEnabled[(int)helpType]);
    }

    /// <summary>
    /// Create a callback for when the help is enabled or disabled.
    /// </summary>
    /// <param name="helpType"></param>
    /// <param name="helpUpdatedDelegate"></param>
    public void SetHelpCallback(HelpType helpType, HelpUpdatedDelegate helpUpdatedDelegate)
    {
        helpUpdatedDelegates[(int)helpType] += helpUpdatedDelegate;
    }

    public void OnHelpButtonClicked()
    {
        // If we're in the settings screen, toggle the settings help based on whether the general help is enabled
        if(uiPlayerSettings.isInSettingsGeneralMenu())
        {
            ToggleHelpType(HelpType.SettingsGeneral);
            SetHelpType(HelpType.SettingsPlayer, false);
            SetHelpType(HelpType.SettingsRotation, false);
            SetHelpType(HelpType.Position, false);
            SetHelpType(HelpType.Rotation, false);
            SetHelpType(HelpType.Situation, false);
        }
        else if(uiPlayerSettings.isInSettingsPlayerMenu())
        {
            SetHelpType(HelpType.SettingsGeneral, false);
            ToggleHelpType(HelpType.SettingsPlayer);
            SetHelpType(HelpType.SettingsRotation, false);
            SetHelpType(HelpType.Position, false);
            SetHelpType(HelpType.Rotation, false);
            SetHelpType(HelpType.Situation, false);
        }
        else if(uiPlayerSettings.isInSettingsRotationMenu())
        {
            SetHelpType(HelpType.SettingsGeneral, false);
            SetHelpType(HelpType.SettingsPlayer, false);
            ToggleHelpType(HelpType.SettingsRotation);
            SetHelpType(HelpType.Position, false);
            SetHelpType(HelpType.Rotation, false);
            SetHelpType(HelpType.Situation, false);
        }
        else
        {
            SetHelpType(HelpType.SettingsGeneral, false);
            SetHelpType(HelpType.SettingsPlayer, false);
            SetHelpType(HelpType.SettingsRotation, false);
            ToggleHelpType(HelpType.Position);
            ToggleHelpType(HelpType.Rotation);
            ToggleHelpType(HelpType.Situation);
        }
    }
}

public enum HelpType
{
    Rotation,
    Position,
    Situation,
    SettingsPlayer,
    SettingsRotation,
    SettingsGeneral,

    Last
}