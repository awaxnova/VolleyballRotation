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


    public Toggle toggleGeneral;

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

    bool isRotationSelected(int rotationNumber)
    {
        switch(rotationNumber)
        {
            case 1:
                return toggleRotation1.isOn;
            case 2:
                return toggleRotation2.isOn;
            case 3:
                return toggleRotation3.isOn;
            case 4:
                return toggleRotation4.isOn;
            case 5:
                return toggleRotation5.isOn;
            case 6:
                return toggleRotation6.isOn;
            default:
                return false;
        }
    }

    public void CalculateUIValues() 
    {
        if (!updateUI)
            return;

        // Calculate the values for the UI elements, based on the selected player and rotation(s).

        // Which player number is selected?
        // If it's 0 then we're using the general settings instead of player settings.
        int playerNumber = 0;
        if (togglePlayer1.isOn)
        {
            playerNumber = 1;
        }
        else if (togglePlayer2.isOn)
        {
            playerNumber = 2;
        }
        else if (togglePlayer3.isOn)
        {
            playerNumber = 3;
        }
        else if (togglePlayer4.isOn)
        {
            playerNumber = 4;
        }
        else if (togglePlayer5.isOn)
        {
            playerNumber = 5;
        }
        else if (togglePlayer6.isOn)
        {
            playerNumber = 6;
        }

        // Which rotation numbers are selected, at least one?  If so, enable the UI elements.
        // If not, disable the UI elements.
        if(playerNumber > 0 && ( toggleRotation1.isOn || toggleRotation2.isOn || toggleRotation3.isOn || toggleRotation4.isOn || toggleRotation5.isOn || toggleRotation6.isOn))
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
