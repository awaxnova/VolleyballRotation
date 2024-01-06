using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static Arrow.AnimatedArrowRenderer;

public class UIPlayerSettings : MonoBehaviour
{

    // Default settings, and per player settings, for
    // name, arrowHead, arrowSegment, segmentLength, and arrowHeight
    // are stored in PlayerPrefs.

    // Reference to the UI elements that display the player settings for name, arrowHead, arrowSegment, segmentLength, and arrowHeight
    // Reference to a set of radio buttons, which correspond to default player settings, and players 1 through 6.
    // When a radio button is selected, the player settings for that player are displayed in the UI elements.
    // When a UI element is modified, the player settings for that player are updated, including saving the player settings to PlayerPrefs.
    // Upon start-up, the default player settings are loaded from PlayerPrefs,
    // and used by the RotationManager to set the names, the arrowHead, arrowSegment, segmentLength, and arrowHeight.  


    // The general settings toggle is used to manage the behavior of the player settings.
    // The Default player settings manage the settings in the Rotation Manager component, that apply to each player, when they're not overridden
    // The Player 1 through 6 player settings manage the settings in the Rotation Manager component, that apply to each player, overriding any default settings.

    // The rotation toggles allow us to choose which rotation overrides to set.  Anything that isn't blank will be written.  If it's blank, we don't touch it.

    // How do we populate the dropdown with enum values?
    // https://forum.unity.com/threads/how-to-populate-a-dropdown-with-enum-values.264449/

    public TMP_InputField nameInputField;
    public Dropdown arrowHeadDropdown;
    public Dropdown arrowSegmentDropdown;
    public Slider segmentLengthSlider;
    public Slider arrowHeightSlider;

    public Toggle toggleGeneral;
    public Toggle togglePlayerDefault;
    public Toggle togglePlayer1;
    public Toggle togglePlayer2;
    public Toggle togglePlayer3;
    public Toggle togglePlayer4;
    public Toggle togglePlayer5;
    public Toggle togglePlayer6;

    public Toggle toggleRotation1;
    public Toggle toggleRotation2;
    public Toggle toggleRotation3;
    public Toggle toggleRotation4;
    public Toggle toggleRotation5;
    public Toggle toggleRotation6;

    void PopulateDropdownWithEnum(Dropdown dropdown, System.Type enumType)
    {
        dropdown.ClearOptions();
        List<string> enumNames = new List<string>(System.Enum.GetNames(enumType));
        dropdown.AddOptions(enumNames);
    }


    // Start is called before the first frame update
    void Start()
    {

        toggleGeneral?.onValueChanged.AddListener(OnValueChangedToggleGeneral);
        togglePlayerDefault?.onValueChanged.AddListener(OnValueChangedTogglePlayerDefault);
        togglePlayer1?.onValueChanged.AddListener(OnValueChangedTogglePlayer1);
        togglePlayer2?.onValueChanged.AddListener(OnValueChangedTogglePlayer2);
        togglePlayer3?.onValueChanged.AddListener(OnValueChangedTogglePlayer3);
        togglePlayer4?.onValueChanged.AddListener(OnValueChangedTogglePlayer4);
        togglePlayer5?.onValueChanged.AddListener(OnValueChangedTogglePlayer5);
        togglePlayer6?.onValueChanged.AddListener(OnValueChangedTogglePlayer6);

        toggleRotation1?.onValueChanged.AddListener(OnValueChangedToggleRotation1);
        toggleRotation2?.onValueChanged.AddListener(OnValueChangedToggleRotation2);
        toggleRotation3?.onValueChanged.AddListener(OnValueChangedToggleRotation3);
        toggleRotation4?.onValueChanged.AddListener(OnValueChangedToggleRotation4);
        toggleRotation5?.onValueChanged.AddListener(OnValueChangedToggleRotation5);
        toggleRotation6?.onValueChanged.AddListener(OnValueChangedToggleRotation6);

        nameInputField.onEndEdit.AddListener((string name) => { Debug.Log($"nameInputField.onEndEdit({name})"); });
        arrowHeadDropdown.onValueChanged.AddListener((int arrowHead) => { Debug.Log($"arrowHeadDropdown.onValueChanged({arrowHead})"); });
        arrowSegmentDropdown.onValueChanged.AddListener((int arrowSegment) => { Debug.Log($"arrowSegmentDropdown.onValueChanged({arrowSegment})"); });
        segmentLengthSlider.onValueChanged.AddListener((float segmentLength) => { Debug.Log($"segmentLengthSlider.onValueChanged({segmentLength})"); });
        arrowHeightSlider.onValueChanged.AddListener((float arrowHeight) => { Debug.Log($"arrowHeightSlider.onValueChanged({arrowHeight})"); });

        // Load the settings from the PlayerPrefs for the selected player.

        PopulateDropdownWithEnum(arrowHeadDropdown, typeof(ArrowTypes));
        PopulateDropdownWithEnum(arrowSegmentDropdown, typeof(SegmentTypes));
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public void UpdateUIValues(string name, ArrowTypes arrowHead, SegmentTypes arrowSegment, float segmentLength, float arrowHeight)
    {

        nameInputField.text = name;
        arrowHeadDropdown.value = (int)arrowHead;
        arrowSegmentDropdown.value = (int)arrowSegment;
        segmentLengthSlider.value = segmentLength;
        arrowHeightSlider.value = arrowHeight;        
    }


    private void HandleToggleSelectedPlayer(int playerNumber)
    { 
        // Load the settings from the PlayerPrefs for the selected player.  
        Debug.Log($"HandleToggleSelectedPlayer({playerNumber})");
    }

    private void HandleToggleSelectedRotation(int rotationNumber)
    {
        // Load the settings from the PlayerPrefs for the selected rotation.  
        Debug.Log($"HandleToggleSelectedRotation({rotationNumber})");
    }

    public void OnValueChangedToggleGeneral(bool isSelected) 
    { 
    
    }

    public void OnValueChangedTogglePlayerDefault(bool isSelected)
    {
    
    }

    #region OnValueChangedTogglePlayer
    public void OnValueChangedTogglePlayer1(bool isSelected)
    {
        if(isSelected)
        {
            HandleToggleSelectedPlayer(1);
        }
    }

    public void OnValueChangedTogglePlayer2(bool isSelected)
    {
        if(isSelected)
        {
            HandleToggleSelectedPlayer(2);
        }
    }

    public void OnValueChangedTogglePlayer3(bool isSelected)
    {
        if(isSelected)
        {
            HandleToggleSelectedPlayer(3);
        }
    }

    public void OnValueChangedTogglePlayer4(bool isSelected)
    {
        if(isSelected)
        {
            HandleToggleSelectedPlayer(4);
        }
    }

    public void OnValueChangedTogglePlayer5(bool isSelected)
    {
        if(isSelected)
        {
            HandleToggleSelectedPlayer(5);
        }
    }

    public void OnValueChangedTogglePlayer6(bool isSelected)
    {
        if(isSelected)
        {
            HandleToggleSelectedPlayer(6);
        }
    }
    #endregion

    #region OnValueChangedToggleRotation
    public void OnValueChangedToggleRotation1(bool isSelected)
    {
        if(isSelected)
        {
            HandleToggleSelectedRotation(1);
        }
    }

    public void OnValueChangedToggleRotation2(bool isSelected)
    {
        if(isSelected)
        {
            HandleToggleSelectedRotation(2);
        }
    }

    public void OnValueChangedToggleRotation3(bool isSelected)
    {
        if(isSelected)
        {
            HandleToggleSelectedRotation(3);
        }
    }

    public void OnValueChangedToggleRotation4(bool isSelected)
    {
        if(isSelected)
        {
            HandleToggleSelectedRotation(4);
        }
    }

    public void OnValueChangedToggleRotation5(bool isSelected)
    {
        if(isSelected)
        {
            HandleToggleSelectedRotation(5);
        }
    }

    public void OnValueChangedToggleRotation6(bool isSelected)
    {
        if(isSelected)
        {
            HandleToggleSelectedRotation(6);
        }
    }
    #endregion

}
