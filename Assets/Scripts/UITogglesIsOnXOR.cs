using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UITogglesIsOnXOR : MonoBehaviour
{
    // Two lists of toggles in a groups that can be on only if the other group is off
    public List<Toggle> toggles1 = new List<Toggle>();

    public List<Toggle> toggles2 = new List<Toggle>();

    // Start is called before the first frame update
    void Start()
    {
        // Add a listener to each toggle in the first group
        foreach (Toggle toggle in toggles1)
        {
            toggle.onValueChanged.AddListener(delegate { OnToggleValueChanged(toggle); });
        }

        // Add a listener to each toggle in the second group
        foreach (Toggle toggle in toggles2)
        {
            toggle.onValueChanged.AddListener(delegate { OnToggleValueChanged(toggle); });
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // When a toggle is changed, check if it is on and if so,
    // turn off all the other toggles in the other group
    void OnToggleValueChanged(Toggle toggle)
    {
        if (toggle.isOn)
        {
            if (toggles1.Contains(toggle))
            {
                foreach (Toggle toggle2 in toggles2)
                {
                    toggle2.isOn = false;
                }
            }
            else if (toggles2.Contains(toggle))
            {
                foreach (Toggle toggle1 in toggles1)
                {
                    toggle1.isOn = false;
                }
            }
        }
    }
   
}
