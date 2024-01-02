using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIScaleAdjuster : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        LoadUIScale();
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void SetUIScale(float scale)
    { 
        gameObject.transform.localScale = Vector3.one * scale;

    }
    public void ScaleUIElement(float newScaleValue)
    {
        Debug.Log($"ScaleUIElement by {newScaleValue}");

        SetUIScale(newScaleValue);

        SaveUIScale(newScaleValue);
    }

    string GetUniqueKey()
    {
        Transform currentTransform = transform;
        string uniqueKey = "";

        while (currentTransform != null)
        {
            uniqueKey = currentTransform.name + "." + uniqueKey;
            currentTransform = currentTransform.parent;
        }

        return uniqueKey.TrimEnd('.') + "_UIScale";
    }

    private void SaveUIScale(float newScaleValue)
    {
        string key = GetUniqueKey();
        PlayerPrefs.SetFloat(key, newScaleValue);
        PlayerPrefs.Save();
    }

    private void LoadUIScale()
    {
        string key = GetUniqueKey();
        if(PlayerPrefs.HasKey(key))
        {
            float savedScale = PlayerPrefs.GetFloat(key);
            SetUIScale(savedScale);
        }
    }
}
