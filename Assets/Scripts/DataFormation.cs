using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VolleyballRotation
{
    [CreateAssetMenu(fileName = "_dataFormation_", menuName = "VolleyballRotation/DataFormation", order = 4)]

    public class DataFormation : ScriptableObject
    {
        /// <summary>
        /// The unique identifier for this formation, such as 6-2
        /// </summary>
        public string id;

        [Header("Text")]
        [InfoBox("Such as a 6-2 Formation for Volleyball")]
        public string title;

        [TextArea(5, 7)]
        public string desc;

        // Data Positions
        public List<DataPosition> dataPositions = new List<DataPosition>();

        public static List<DataFormation> data_formation_list = new List<DataFormation>();

        public static void Load(string folder = "")
        {
            if (data_formation_list.Count == 0)
                data_formation_list.AddRange(Resources.LoadAll<DataFormation>(folder));
        }


    }

    public enum Formation
    {
        F6_2,
        None,
    }
}
