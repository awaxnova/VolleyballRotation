using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class HelpFilter : MonoBehaviour
{

    /// <summary>
    /// This method should return the GameObject that we want to filter for.
    /// </summary>
    /// <returns></returns>
    public abstract GameObject filterForGameObject ();
    
    public Vector3 GetPosition()
    {
        return filterForGameObject().transform.position;
    }   
}
