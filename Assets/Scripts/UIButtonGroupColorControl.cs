using System.Collections;
using System.Collections.Generic;
using TheraBytes.BetterUi;
using UnityEngine;
using UnityEngine.UI;

public class UIButtonGroupColorControl : MonoBehaviour
{

    // a list of all of the buttons in the group
    public List<Button> buttonList;

    public Color defaultColor = Color.white;

    // Start is called before the first frame update
    void Start()
    {
        // Set all transitions to None
        foreach (Button button in buttonList)
        {
            // Set the image color to the default color.
            button.image.color = defaultColor;

            button.transition = Selectable.Transition.ColorTint;

            ColorBlock colorBlock = button.colors;
            colorBlock.normalColor = Color.white;
            colorBlock.selectedColor = Color.white;
            colorBlock.disabledColor = Color.white;
            colorBlock.pressedColor = Color.white;
            colorBlock.highlightedColor = Color.white;
            button.colors = colorBlock;

            button.transition = Selectable.Transition.None;

        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetNormalColor(Color color, int index)
    {
        //ColorBlock colorBlock = buttonList[index].colors;
        //colorBlock.normalColor = color;
        //buttonList[index].colors = colorBlock;

        buttonList[index].image.color = color;

        //buttonList[index].colors.normalColor= color;
    }

    public void SetColorSelection(int selectedIndex, Color selectedColor, int altSelectedIndex, Color altSelectedColor, Color notSelectedColor)
    {
        for (int i = 0; i < buttonList.Count; i++)
        {
            if (i == selectedIndex)
            {
                SetNormalColor(selectedColor, i);
            }
            else if(i == altSelectedIndex)
            {
                SetNormalColor(altSelectedColor, i);
            }
            else
            {
                SetNormalColor(notSelectedColor, i);
            }
        }
    }
}
