using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIToggleActive : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnToggleActive()
    {       
        gameObject.SetActive(!gameObject.activeSelf);
    }
}
