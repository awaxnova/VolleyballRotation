using Arrow;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VolleyballRotation;
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

    public RotationManager rotationManager;

    public TMP_InputField nameInputField;
    public Dropdown arrowHeadDropdown;
    public Dropdown arrowSegmentDropdown;
    public Slider segmentLengthSlider;
    public Slider arrowHeightSlider;

    [Header("Toggles for choosing the General Settings panel of options")]
    public Toggle toggleGeneral;

    [Header("Toggles for which player is selected - a maximum of one selected at a time.")]
    public Toggle[] togglePlayers = new Toggle[6];

    [Header("Toggles for which rotations in this formation will be affected by the settings")]
    public Toggle[] toggleRotations = new Toggle[6];
          
    bool updateUI = true;

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

        togglePlayers[0]?.onValueChanged.AddListener(OnValueChangedTogglePlayer1);
        togglePlayers[1]?.onValueChanged.AddListener(OnValueChangedTogglePlayer2);
        togglePlayers[2]?.onValueChanged.AddListener(OnValueChangedTogglePlayer3);
        togglePlayers[3]?.onValueChanged.AddListener(OnValueChangedTogglePlayer4);
        togglePlayers[4]?.onValueChanged.AddListener(OnValueChangedTogglePlayer5);
        togglePlayers[5]?.onValueChanged.AddListener(OnValueChangedTogglePlayer6);

        toggleRotations[0]?.onValueChanged.AddListener(OnValueChangedToggleRotation1);
        toggleRotations[1]?.onValueChanged.AddListener(OnValueChangedToggleRotation2);
        toggleRotations[2]?.onValueChanged.AddListener(OnValueChangedToggleRotation3);
        toggleRotations[3]?.onValueChanged.AddListener(OnValueChangedToggleRotation4);
        toggleRotations[4]?.onValueChanged.AddListener(OnValueChangedToggleRotation5);
        toggleRotations[5]?.onValueChanged.AddListener(OnValueChangedToggleRotation6);

        nameInputField.onEndEdit.AddListener((string name) => { Debug.Log($"nameInputField.onEndEdit({name})"); });
        arrowHeadDropdown.onValueChanged.AddListener((int arrowHead) => { Debug.Log($"arrowHeadDropdown.onValueChanged({arrowHead})"); });
        arrowSegmentDropdown.onValueChanged.AddListener((int arrowSegment) => { Debug.Log($"arrowSegmentDropdown.onValueChanged({arrowSegment})"); });
        segmentLengthSlider.onValueChanged.AddListener((float segmentLength) => { Debug.Log($"segmentLengthSlider.onValueChanged({segmentLength})"); });
        arrowHeightSlider.onValueChanged.AddListener((float arrowHeight) => { Debug.Log($"arrowHeightSlider.onValueChanged({arrowHeight})"); });

        // Load the settings from the PlayerPrefs for the selected player.

        PopulateDropdownWithEnum(arrowHeadDropdown, typeof(ArrowTypes));
        PopulateDropdownWithEnum(arrowSegmentDropdown, typeof(SegmentTypes));

        nameInputField.onSubmit.AddListener(OnSubmitInputFieldName); 
        arrowHeadDropdown.onValueChanged.AddListener(OnValueChangedArrowHeadDropdown);
        arrowSegmentDropdown.onValueChanged.AddListener(OnValueChangedArrowSegmentDropdown);
        segmentLengthSlider.onValueChanged.AddListener(OnValueChangedSegmentLengthSlider);
        arrowHeightSlider.onValueChanged.AddListener(OnValueChangedArrowHeightSlider);

        // prevent interpretation of rich text tags.
        nameInputField.richText = false;
    }

    private void OnSubmitInputFieldName(string value)
    {
        Debug.Log($"OnSubmitInputFieldName({value})");
        // Update the data structure with the new name.
    }

    private void OnValueChangedArrowHeadDropdown(int value)
    {
        Debug.Log($"OnValueChangedArrowHeadDropdown({(AnimatedArrowRenderer.ArrowTypes)value})");
    }

    private void OnValueChangedArrowSegmentDropdown(int value)
    {
        Debug.Log($"OnValueChangedArrowSegmentDropdown({(AnimatedArrowRenderer.SegmentTypes)value})");
    }

    private void OnValueChangedSegmentLengthSlider(float value)
    {
        Debug.Log($"OnValueChangedSegmentLengthSlider({value})");
    }

    private void OnValueChangedArrowHeightSlider(float value)
    {
        Debug.Log($"OnValueChangedArrowHeightSlider({value})");
    }

    // Update is called once per frame
    void Update()
    {
        CalculateUIValues();
        
    }

    public void EnableUIElements(bool enable)
    { 
        nameInputField.gameObject.SetActive(enable);
        arrowHeadDropdown.gameObject.SetActive(enable);
        arrowSegmentDropdown.gameObject.SetActive(enable);
        segmentLengthSlider.gameObject.SetActive(enable);
        arrowHeightSlider.gameObject.SetActive(enable);
    }

    // Check if  particular rotation number is ON.
    bool isRotationSelected(int rotationNumber)
    {
        if (toggleRotations[rotationNumber-1].isOn)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    int getPlayerNumberToggleSelected()
    {
        for(int indexer=0; indexer < togglePlayers.Length; indexer++)
        {
            if (togglePlayers[indexer].isOn)
            {
                return indexer + 1;
            }
        }

        return 0;
    }

    int getQuantityRotationTogglesSelected()
    {
        int count = 0;
        for (int indexer = 0; indexer < toggleRotations.Length; indexer++)
        {
            if (toggleRotations[indexer].isOn)
            {
                count++;
            }
        }

        return count;
    }

    public void CalculateUIValues() 
    {
        if (!updateUI)
            return;

        // Calculate the values for the UI elements, based on the selected player and rotation(s).

        // Which player number is selected?
        // If it's 0 then we're using the general settings instead of player settings.
        int playerNumber = getPlayerNumberToggleSelected();

        // Which rotation numbers are selected, at least one?  If so, enable the UI elements.
        // If not, disable the UI elements.
        if((playerNumber > 0) && (getQuantityRotationTogglesSelected() > 0))
        {
            EnableUIElements(true);
        }
        else
        {
            EnableUIElements(false);
            return;
        }


        // Let's load the UI Elements with the values that are common to selected rotations.
        // One player and at least one rotation is selected.

        FormationData formationData = rotationManager.GetCurrentFormationData();

        // For the selected player, diff the values for the rotations/situations.
        // If they're all the same, display the value.
        // If they're not all the same, display a blank.

        if (formationData != null)
        {

            // Test all of the situations for the rotation number, for this player, and save off the values, but blank if they're different.
            string name = "";
            ArrowTypes arrowHead = ArrowTypes.None;
            SegmentTypes arrowSegment = SegmentTypes.None;
            float segmentLength = 0;
            float arrowHeight = 0;

            bool first = true;

            // Iterate over all Situation types
            foreach(Situation situation in System.Enum.GetValues(typeof(Situation)))
            {
                // Iterate over all SELECTED Rotation numbers
                for (int rotationNumber = 1; rotationNumber <= 6; rotationNumber++)
                {
                    RotationData rotationData = formationData.GetRotationData(situation, rotationNumber);

                    // If the rotation /situation, then compare the values.
                    if ( isRotationSelected(rotationNumber) && (rotationData != null))
                    {
                        if (first)
                        {
                            name = formationData.GetPlayerName(situation, rotationNumber, playerNumber);
                            arrowHead = formationData.GetArrowType(situation, rotationNumber, playerNumber);
                            arrowSegment = formationData.GetSegmentType(situation, rotationNumber, playerNumber);
                            segmentLength = formationData.GetArrowSegmentLength(situation, rotationNumber, playerNumber);
                            arrowHeight = formationData.GetArrowHeight(situation, rotationNumber, playerNumber);
                            first = false;
                        }
                        else
                        {
                            if (name != formationData.GetPlayerName(situation, rotationNumber, playerNumber))
                            {
                                name = "";
                            }
                            if (arrowHead != formationData.GetArrowType(situation, rotationNumber, playerNumber))
                            {
                                arrowHead = ArrowTypes.None;
                            }
                            if (arrowSegment != formationData.GetSegmentType(situation, rotationNumber, playerNumber))
                            {
                                arrowSegment = SegmentTypes.None;
                            }
                            if (segmentLength != formationData.GetArrowSegmentLength(situation, rotationNumber, playerNumber))
                            {
                                segmentLength = 0;
                            }
                            if (arrowHeight != formationData.GetArrowHeight(situation, rotationNumber, playerNumber))
                            {
                                arrowHeight = 0;
                            }
                        }
                    }
                }
            }

            UpdateUIValues(name, arrowHead, arrowSegment, segmentLength, arrowHeight);
        }

        updateUI = false;
    }

    public void UpdateUIValues(string name, ArrowTypes arrowHead, SegmentTypes arrowSegment, float segmentLength, float arrowHeight)
    {

        nameInputField.text = name;
        arrowHeadDropdown.value = (int)arrowHead;
        arrowSegmentDropdown.value = (int)arrowSegment;
        segmentLengthSlider.value = segmentLength;
        arrowHeightSlider.value = arrowHeight;        
    }


    private void HandleToggleChangedPlayer(int playerNumber, bool isSelected)
    { 
        //Debug.Log($"HandleToggleSelectedPlayer({playerNumber} {isSelected})");
        updateUI = true;
    }

    private void HandleToggleChangedRotation(int rotationNumber, bool isSelected)
    {
        updateUI = true;
    }
        

    public void OnValueChangedToggleGeneral(bool isSelected) 
    {
        updateUI = true;
    }

    #region OnValueChangedTogglePlayer
    public void OnValueChangedTogglePlayer1(bool isSelected)
    {
        HandleToggleChangedPlayer(1, isSelected);
    }

    public void OnValueChangedTogglePlayer2(bool isSelected)
    {
        HandleToggleChangedPlayer(2, isSelected);
    }

    public void OnValueChangedTogglePlayer3(bool isSelected)
    {
        HandleToggleChangedPlayer(3, isSelected);
    }

    public void OnValueChangedTogglePlayer4(bool isSelected)
    {
        HandleToggleChangedPlayer(4, isSelected);
    }

    public void OnValueChangedTogglePlayer5(bool isSelected)
    {
        HandleToggleChangedPlayer(5, isSelected);
    }

    public void OnValueChangedTogglePlayer6(bool isSelected)
    {
        HandleToggleChangedPlayer(6, isSelected);
    }
    #endregion

    #region OnValueChangedToggleRotation
    public void OnValueChangedToggleRotation1(bool isSelected)
    {
        HandleToggleChangedRotation(1, isSelected);
    }

    public void OnValueChangedToggleRotation2(bool isSelected)
    {
        HandleToggleChangedRotation(2, isSelected);
    }

    public void OnValueChangedToggleRotation3(bool isSelected)
    {
        HandleToggleChangedRotation(3, isSelected);
    }

    public void OnValueChangedToggleRotation4(bool isSelected)
    {
        HandleToggleChangedRotation(4, isSelected);
    }

    public void OnValueChangedToggleRotation5(bool isSelected)
    {
        HandleToggleChangedRotation(5, isSelected);
    }

    public void OnValueChangedToggleRotation6(bool isSelected)
    {
        HandleToggleChangedRotation(6, isSelected);
    }
    #endregion

}
