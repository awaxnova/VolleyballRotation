using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIToggleActive : MonoBehaviour
{

    public bool initialState = false;

    // Start is called before the first frame update
    void Start()
    {
        gameObject.SetActive(initialState);

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
