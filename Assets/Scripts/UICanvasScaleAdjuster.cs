using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UICanvasScaleAdjuster : MonoBehaviour
{
    private CanvasScaler canvasScaler;

    // Start is called before the first frame update
    void Start()
    {
        // Get a reference to the canvas scaler
        canvasScaler = GetComponent<CanvasScaler>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetCanvasScale(float newScaleValue)
    {

        Vector2 scaledVal = canvasScaler.referenceResolution;
        scaledVal.Scale(new Vector2(newScaleValue, newScaleValue));
        
        canvasScaler.referenceResolution = scaledVal;

        Debug.Log($"Canvas new scale: {newScaleValue} to {canvasScaler.referenceResolution} by {scaledVal}");

    }
}
