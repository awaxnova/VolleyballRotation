using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public abstract class HelpFilter : MonoBehaviour
{

    /// <summary>
    /// This method should return the GameObject that we want to filter for.
    /// </summary>
    /// <returns></returns>
    public abstract GameObject filterForGameObject ();
    
    public Vector3 GetPosition(GameObject target)
    {
        //return filterForGameObject().transform.position;

        var source = filterForGameObject();

        return CalcTargetPositionToMatchSource(source, target);
        // Maybe instead of GetPosition, let's operate on the source and target objects, making the target object's pivot match the screen space coordinate of the source object.
    }

    private Vector3 CalcTargetPositionToMatchSource(GameObject source, GameObject target)
    {
        // Given the position of the source object in screen space,
        // calculate the position to set the target object to,
        // so that it's screen space coordinates match those of the source object.
        // This is so that we can set the target object's pivot to match the screen
        // space coordinates of the source object.

        // Get the RectTransform of the source object
        RectTransform sourceRectTransform = source.GetComponent<RectTransform>();
        Assert.IsTrue(sourceRectTransform != null);

        // Get the RectTransform of the target object
        RectTransform targetRectTransform = target.GetComponent<RectTransform>();
        Assert.IsTrue(targetRectTransform != null);

        /*
        // Get the screen space coordinates of the source object
        Vector3[] sourceCorners = new Vector3[4];
        sourceRectTransform.GetWorldCorners(sourceCorners);
        Vector3 sourceScreenSpacePosition = Camera.main.WorldToScreenPoint(sourceCorners[0]);

        // Get the screen space coordinates of the target object
        Vector3[] targetCorners = new Vector3[4];
        targetRectTransform.GetWorldCorners(targetCorners);
        Vector3 targetScreenSpacePosition = Camera.main.WorldToScreenPoint(targetCorners[0]);

        // Calculate the difference between the two screen space coordinates
        Vector3 difference = sourceScreenSpacePosition - targetScreenSpacePosition;

        // Add the difference to the target object's position
        Vector3 targetPosition = target.transform.position + difference;
        
        // Return the new position
        return targetPosition;
        */

        Vector3 sourceWorldPos = sourceRectTransform.position;
        Vector3 targetLocalPos = targetRectTransform.parent.InverseTransformPoint(sourceWorldPos);

        //targetRectTransform.anchoredPosition = targetLocalPos;

        return targetLocalPos;
    }
}
