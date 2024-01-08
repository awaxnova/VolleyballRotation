using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// This will manage a set of toggle enables, based on how many of other toggles are set.
public class UITogglesEnableObjs : MonoBehaviour
{
    // A list of toggles, for which when at least a threshold of them are ON, the toggles in the other list are enabled.
    public List<Toggle> toggles;
    public List<GameObject> enableObjs;
    public int threshold = 1;
    private int count = 0;
    private int lastCount = -1;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {               
        count = 0;
        foreach (Toggle toggle in toggles)
        {
            if (toggle.isOn)
            {
                count++;
            }
        }

        if (count != lastCount)
        {
            lastCount = count;
            UpdateEnableObjs();
        }
    }

    void UpdateEnableObjs()
    {
        if (count >= threshold)
        {
            foreach (GameObject obj in enableObjs)
            {
                obj.SetActive(true);
            }
        }
        else
        {
            foreach (GameObject obj in enableObjs)
            {
                obj.SetActive(false);
            }
        }
    }
}
