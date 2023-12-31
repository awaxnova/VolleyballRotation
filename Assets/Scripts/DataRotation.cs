using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
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

}

