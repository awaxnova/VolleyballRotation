using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text.RegularExpressions;
using UnityEngine;

namespace VolleyballRotation
{

    /*
     TODO - Make a finder, to populate the rotation based on position attributes: situation/rotationIndex/formation
     */

    [CreateAssetMenu(fileName = "_dataRotation_", menuName = "VolleyballRotation/DataRotation", order = 3)]
    public class DataRotation : ScriptableObject
    {
        public Formation formation;

        // 1 through 6, corresponding to rotation Numbers 1 through 6, corresponding to rotation indices 0 through 5
        [Range(1, 6)]
        public int rotationNumber;

        [TableList(ShowIndexLabels = true, AlwaysExpanded = true)]
        public List<DataPosition> dataPositions = new List<DataPosition>();


        public static List<DataRotation> data_list = new List<DataRotation>();

        public static void Load(string folder = "")
        {
            if (data_list.Count == 0)
                data_list.AddRange(Resources.LoadAll<DataRotation>(folder));
        }

        [Button("PopulateRotations", Name = "Update")]
        private void PopulateRotations()
        { 
            // From the DataPosition static list, populate the dataPositions that match this rotation and formation.
            dataPositions.Clear();

            if(DataPosition.data_list.Count == 0)
                DataPosition.Load();

            foreach (DataPosition dataPosition in DataPosition.data_list)
            {
                if (dataPosition.rotationNumber == rotationNumber && dataPosition.formation == formation)
                {
                    dataPositions.Add(dataPosition);
                }
            }
        }
    }


    public enum Situation
    {
        Rotation,
        ServeStack,
        //TransitionToDefense,
        //BasePositionForDefense,
        ServeReceiveStack,
        //TransitionToAttack,
        ReadyToAttack,
        //HitterCoverageReady,    // Hitter Coverage can show a map from current position, to the destination positions based on which hitter they're defending.
        //HitterCoverageLeft,
        //HitterCoverageMid,
        //HitterCoverageRight,
        //SwitchToDefense,
        BaseDefense,
        None,
    }


    // Create a custom drawer for the Situation enum using Odin
    public class SituationEnumDrawer : OdinValueDrawer<Situation>
    {
        protected override void DrawPropertyLayout(GUIContent label)
        {
            Color origBGColor = GUI.backgroundColor;

            GUILayout.BeginHorizontal();

            foreach (Situation enumValue in System.Enum.GetValues(typeof(Situation)))
            {
                if(enumValue == Situation.None)
                    continue;

                bool isSelected = ValueEntry.SmartValue.Equals(enumValue);
                GUI.backgroundColor = isSelected ? Color.green : Color.white;

                if (GUILayout.Button(enumValue.ToString().SplitCamelCase(), GUILayout.ExpandWidth(false)))
                {
                    //ValueEntry.SmartValue = enumValue;

                    // Call the method dynamically based on the enum value
                    InvokeMethod(enumValue, ValueEntry.SmartValue, ValueEntry);
                }
            }

            GUI.backgroundColor = origBGColor;

            GUILayout.EndHorizontal();
        }

        private void InvokeMethod(Situation clickedEnumValue, Situation originalValue, IPropertyValueEntry<Situation> valueEntry)
        {
            //string methodName = "On" + clickedEnumValue.ToString().SplitCamelCase() + "Clicked";
            string handlerName = "OnSituationClickedHandler";

            Type containingType = typeof(RotationManager); // Replace YourClassContainingMethods with the actual class name where methods are defined
            //MethodInfo methodInfo = containingType.GetMethod(methodName);
            MethodInfo handlerInfo = containingType.GetMethod(handlerName);
            /*
            if (methodInfo != null)
            {
                // If the method exists, invoke it
                methodInfo.Invoke(null, new object[] { clickedEnumValue, originalValue});
            }
            else
            {
                Debug.LogWarning("Method " + methodName + " not found!");
            }
            */
            if (handlerInfo != null)
            {
                var instance = GameObject.FindObjectOfType<RotationManager>();
                // If the method exists, invoke it
                handlerInfo.Invoke(instance, new object[] { clickedEnumValue, originalValue });
            }
            else
            {
                Debug.LogWarning("Method " + handlerName + " not found!");
            }
            
            valueEntry.SmartValue = clickedEnumValue;

        }
    }

}


public static class StringExtensions
{
    public static string SplitCamelCase(this string input)
    {
        return Regex.Replace(input, "(?<=[a-z])([A-Z])", " $1", RegexOptions.Compiled);
    }
}

