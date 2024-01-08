using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIInputField : MonoBehaviour
{
    public TMP_InputField inputField;

    private void Start()
    {
    
        // Get the input field component
        inputField = GetComponent<TMP_InputField>();

        // Add listener to detect when the input field is submitted
        //inputField.onEndEdit.AddListener(SubmitInput);

        inputField.onFocusSelectAll = true;
        inputField.onSelect.AddListener(FocusInputField);
    
    }
    // Example method to focus the input field
    public void FocusInputField(string value)
    {
        inputField.Select();
        inputField.ActivateInputField();
    }
}