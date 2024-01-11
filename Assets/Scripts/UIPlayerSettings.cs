using Arrow;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

    public InputField nameInputField;
    public Dropdown arrowHeadDropdown;
    public Dropdown arrowSegmentDropdown;
    public Slider segmentLengthSlider;
    public Slider arrowHeightSlider;
    public FlexibleColorPicker arrowHeadColorPicker;
    public FlexibleColorPicker arrowSegmentColorPicker;
    public FlexibleColorPicker courtFrontColorPicker;
    public FlexibleColorPicker courtBackColorPicker;

    [Header("Toggles for choosing the General Settings panel of options")]
    public Toggle toggleGeneral;

    [Header("Toggles for which player is selected - a maximum of one selected at a time.")]
    public Toggle[] togglePlayers = new Toggle[6];

    [Header("Toggles for which rotations in this formation will be affected by the settings")]
    public Toggle[] toggleRotations = new Toggle[6];

    public GameObject ButtonSettingsRevert;

    bool updateUI = true;

    bool enteringSettingsMenu = false;
    bool isDirtySettings = false;

    void PopulateDropdownWithEnum(Dropdown dropdown, System.Type enumType)
    {
        dropdown.ClearOptions();
        List<string> enumNames = new List<string>(System.Enum.GetNames(enumType));
        dropdown.AddOptions(enumNames);
    }

    [Header("This is the Gameobject that makes up the court floor, which has two materials, [0] is for the back court, and [1] is for the front court.")]
    public GameObject courtFloor;



    private void OnEnable()
    {
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

        arrowHeadColorPicker.onColorChanged.AddListener(OnColorChangedArrowHeadColorPicker);
        arrowSegmentColorPicker.onColorChanged.AddListener(OnColorChangedArrowSegmentColorPicker);

        LoadCourtColors();        
        courtBackColorPicker.onColorChanged.AddListener(SetBackCourtColor);
        courtFrontColorPicker.onColorChanged.AddListener(SetFrontCourtColor);

        // prevent interpretation of rich text tags.
        //nameInputField.richText = false;

        ButtonSettingsRevert.gameObject.SetActive(false); // Hide the revert button initially.
    }

    private void OnColorChangedArrowHeadColorPicker(Color color)
    {
        Debug.Log($"OnColorChangedColorPicker({color})");

        // Set the color of the dropdown to the color of the arrowHead.
        arrowHeadDropdown.GetComponent<Image>().color = color;

        for(int rotationNumber = 1; rotationNumber <= 6; rotationNumber++)
        {
            if (isRotationSelected(rotationNumber))
            {
                foreach (Situation situation in System.Enum.GetValues(typeof(Situation)).Cast<Situation>().Where(s => s != Situation.None))
                {
                    // Update the arrowHeadColor for this rotation.
                    rotationManager.GetCurrentFormationData().SetArrowHeadColor(situation, rotationNumber, getPlayerNumberToggleSelected(), color);
                }
            }
        }

        SetFlagDirtySettings(true);
    }

    private void OnColorChangedArrowSegmentColorPicker(Color color)
    {
        Debug.Log($"OnColorChangedColorPicker({color})");

        // Set the color of the dropdown to the color of the arrowSegment.
        arrowSegmentDropdown.GetComponent<Image>().color = color;

        for(int rotationNumber = 1; rotationNumber <= 6; rotationNumber++)
        {
            if (isRotationSelected(rotationNumber))
            {
                foreach (Situation situation in System.Enum.GetValues(typeof(Situation)).Cast<Situation>().Where(s => s != Situation.None))
                {
                    // Update the arrowSegmentColor for this rotation.
                    rotationManager.GetCurrentFormationData().SetArrowSegmentColor(situation, rotationNumber, getPlayerNumberToggleSelected(), color);
                }
            }
        }

        SetFlagDirtySettings(true);
    }

    private void OnSubmitInputFieldName(string value)
    {
        Debug.Log($"OnSubmitInputFieldName({value})");
        // Update the data structure with the new name.


        // Use the value to update the data structure.
        // Iterate over all of the selected rotations, and update the name of the player for each of them.

        for(int rotationNumber = 1; rotationNumber <= 6; rotationNumber++)
        {
            if (isRotationSelected(rotationNumber))
            {
                foreach (Situation situation in System.Enum.GetValues(typeof(Situation)).Cast<Situation>().Where(s => s != Situation.None))
                {
                    // Update the name for this rotation.
                    rotationManager.GetCurrentFormationData().SetPlayerName(situation, rotationNumber, getPlayerNumberToggleSelected(), value);
                }
            }
        }

        SetFlagDirtySettings(true);
    }

    private void OnValueChangedArrowHeadDropdown(int value)
    {
        Debug.Log($"OnValueChangedArrowHeadDropdown({(AnimatedArrowRenderer.ArrowTypes)value})");

        for(int rotationNumber = 1; rotationNumber <= 6; rotationNumber++)
        {
            if (isRotationSelected(rotationNumber))
            {
                foreach (Situation situation in System.Enum.GetValues(typeof(Situation)).Cast<Situation>().Where(s => s != Situation.None))
                {
                    // Update the arrowHead for this rotation.
                    rotationManager.GetCurrentFormationData().SetArrowType(situation, rotationNumber, getPlayerNumberToggleSelected(), (AnimatedArrowRenderer.ArrowTypes)value);                   
                }
            }
        }

        SetFlagDirtySettings(true);
    }

    private void OnValueChangedArrowSegmentDropdown(int value)
    {
        Debug.Log($"OnValueChangedArrowSegmentDropdown({(AnimatedArrowRenderer.SegmentTypes)value})");

        for(int rotationNumber = 1; rotationNumber <= 6; rotationNumber++)
        {
            if (isRotationSelected(rotationNumber))
            {
                foreach (Situation situation in System.Enum.GetValues(typeof(Situation)).Cast<Situation>().Where(s => s != Situation.None))
                {
                    // Update the arrowSegment for this rotation.
                    rotationManager.GetCurrentFormationData().SetSegmentType(situation, rotationNumber, getPlayerNumberToggleSelected(), (AnimatedArrowRenderer.SegmentTypes)value);
                }
            }
        }

        SetFlagDirtySettings(true);
    }

    private void OnValueChangedSegmentLengthSlider(float value)
    {
        Debug.Log($"OnValueChangedSegmentLengthSlider({value})");

        for(int rotationNumber = 1; rotationNumber <= 6; rotationNumber++)
        {
            if (isRotationSelected(rotationNumber))
            {
                foreach (Situation situation in System.Enum.GetValues(typeof(Situation)).Cast<Situation>().Where(s => s != Situation.None))
                {
                    // Update the segmentLength for this rotation.
                    rotationManager.GetCurrentFormationData().SetArrowSegmentLength(situation, rotationNumber, getPlayerNumberToggleSelected(), value);
                }
            }
        }

        SetFlagDirtySettings(true);
    }

    private void OnValueChangedArrowHeightSlider(float value)
    {
        Debug.Log($"OnValueChangedArrowHeightSlider({value})");

        for(int rotationNumber = 1; rotationNumber <= 6; rotationNumber++)
        {
            if (isRotationSelected(rotationNumber))
            {
                foreach (Situation situation in System.Enum.GetValues(typeof(Situation)).Cast<Situation>().Where(s => s != Situation.None))
                {
                    // Update the arrowHeight for this rotation.
                    rotationManager.GetCurrentFormationData().SetArrowHeight(situation, rotationNumber, getPlayerNumberToggleSelected(), value);
                }
            }
        }

        SetFlagDirtySettings(true);
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
        arrowHeadColorPicker.gameObject.SetActive(enable);
        arrowSegmentColorPicker.gameObject.SetActive(enable);
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
            Color arrowHeadColor = Color.black;
            Color arrowSegmentColor = Color.black;

            bool first = true;

            // Iterate over all Situation types, where the situation is not None.
            foreach(Situation situation in System.Enum.GetValues(typeof(Situation)).Cast<Situation>().Where(s => s != Situation.None))
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
                            arrowHeadColor = formationData.GetArrowHeadColor(situation, rotationNumber, playerNumber);
                            arrowSegmentColor = formationData.GetArrowSegmentColor(situation, rotationNumber, playerNumber);

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
                            if( arrowHeadColor != formationData.GetArrowHeadColor(situation, rotationNumber, playerNumber))
                            {
                                arrowHeadColor = Color.red;
                            }
                            if (arrowSegmentColor != formationData.GetArrowSegmentColor(situation, rotationNumber, playerNumber))
                            {
                                arrowSegmentColor = Color.blue;
                            }
                        }
                    }
                }
            }

            UpdateUIValues(name, arrowHead, arrowSegment, segmentLength, arrowHeight, arrowHeadColor, arrowSegmentColor);
        }

        updateUI = false;
    }

    public void UpdateUIValues(string name, ArrowTypes arrowHead, SegmentTypes arrowSegment, float segmentLength, float arrowHeight, Color arrowHeadColor, Color arrowSegmentColor)
    {

        nameInputField.text = name;
        arrowHeadDropdown.value = (int)arrowHead;
        arrowSegmentDropdown.value = (int)arrowSegment;
        segmentLengthSlider.value = segmentLength;
        arrowHeightSlider.value = arrowHeight;
        arrowHeadColorPicker.color = arrowHeadColor;
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

    public void SetFrontCourtColor(Color color)
    {
        Debug.Log($"SetFrontCourtColor({color})");

        // Set the Color for the front court color in material 1.

        // Get the materials from the court floor.
        Material[] materials = courtFloor.GetComponent<MeshRenderer>().sharedMaterials;

        // Set the Albedo for the front court color in material 1.
        materials[1].SetColor("_Color", color);

        rotationManager.GetCurrentFormationData().frontCourtColor = color;

        SetFlagDirtySettings(true);

    }


    public void SetBackCourtColor(Color color)
    {

        Debug.Log($"SetBackCourtColor({color})");

        // Set the Color  for the back court color in material 0.

        // Get the materials from the court floor.
        Material[] materials = courtFloor.GetComponent<MeshRenderer>().sharedMaterials;

        // Set the Albedo for the back court color in material 0.
        materials[0].SetColor("_Color", color);

        rotationManager.GetCurrentFormationData().backCourtColor = color;

        SetFlagDirtySettings(true);

    }

    private void SetCourtColors(Color frontCourtColor, Color backCourtColor)
    {
        SetFrontCourtColor(frontCourtColor);
        SetBackCourtColor(backCourtColor);
    }

    private void LoadCourtColors()
    {
        Debug.Log($"LoadCourtColors()");

        Material[] materials = courtFloor.GetComponent<MeshRenderer>().sharedMaterials;

        courtFrontColorPicker.color = materials[1].GetColor("_Color");
        courtBackColorPicker.color = materials[0].GetColor("_Color");
    }

    /// <summary>
    /// This should save/commit any changes to the next snapshot.
    /// </summary>
    public void OnClickedButtonSettings()
    {
        enteringSettingsMenu = !enteringSettingsMenu;

        string buttonText = enteringSettingsMenu ? "ENTERING SETTINGS" : "EXITING SETTINGS";
        Debug.Log($"OnClickedButtonSettings() {enteringSettingsMenu}");

        // if we're entering the settings menu, then take a snapshot of the formation data.
        if (enteringSettingsMenu)
        {
            Debug.Log($"OnClickedButtonSettings() TAKING SNAPSHOT");
            rotationManager.GetCurrentFormationData().Snapshot();
        }
        else if (isDirtySettings)
        {
            // if we're exiting the settings menu, and the settings are dirty, then save the formation data.
            Debug.Log($"OnClickedButtonSettings() SAVING SETTINGS");
            rotationManager.GetCurrentFormationData().Save(forceSave: false);
            SetFlagDirtySettings(false);
        }
    }

    /// <summary>
    /// This should revert any settings before exiting via the settings button.
    /// </summary>
    public void OnClickedButtonRevertSettings() 
    {
        Debug.Log($"OnClickedButtonRevertSettings() REVERTING SETTINGS");
        rotationManager.GetCurrentFormationData().Revert();
        SetFlagDirtySettings(false);
    }

    public void OnClickedButtonFactoryReset()
    {
        // Revert any settings, reload the FormationData from the ScriptableObjects, and clear saved PlayerPrefs.

        rotationManager.FactoryResetFormation();
    }

    private void SetFlagDirtySettings(bool value)
    {

        if (!value) // Setting dirty to false.
            ButtonSettingsRevert.gameObject.SetActive(false);            // Hide Revert Button
        else if (value && !isDirtySettings) // Setting dirty to true.
            ButtonSettingsRevert.gameObject.SetActive(true);             // Show Revert Button 

        isDirtySettings = value;
    }
}
