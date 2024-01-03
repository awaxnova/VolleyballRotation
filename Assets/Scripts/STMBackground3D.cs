using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class STMBackground3D : MonoBehaviour
{

    SuperTextMesh stm;
    MeshFilter bg;

    // Start is called before the first frame update
    void Start()
    {
        // Get a reference to the SuperTextMesh in the parent gameobject
        
        stm = GetComponentInParent<SuperTextMesh>();
        bg = GetComponent<MeshFilter>();
    }

    // Update is called once per frame
    void Update()
    {
        // Scale the Quad to be N times the number of characters in the text.
        // This is a hack to make the background scale with the text.

        // Get the number of characters in the text
        //int numChars = stm.Text.Length;

        //// Get the size of the text
        //Vector2 textSize = new Vector2 (stm.transform.localScale.x, stm.transform.localScale.y);

        //// Get the size of the background
        //Vector2 bgSize = new Vector2 (bg.transform.localScale.x, bg.transform.localScale.y);

        //// Calculate the new size of the background
        //Vector2 newBgSize = new Vector2 (textSize.x * numChars, textSize.y);

        //// Set the new size of the background
        //bg.transform.localScale = new Vector3 (newBgSize.x, newBgSize.y, bg.transform.localScale.z);

        //// Set the new position of the background
        //bg.transform.localPosition = new Vector3 (newBgSize.x / 2, newBgSize.y / 2, bg.transform.localPosition.z);
        


        
        
    }
}
