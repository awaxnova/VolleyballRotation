using Sirenix.OdinInspector;
using Sirenix.Utilities;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VolleyballRotation
{
    [CreateAssetMenu(fileName = "_dataPosition_", menuName = "VolleyballRotation/DataPosition", order = 5)]

    public class DataPosition : ScriptableObject
    {
        /*
        /// <summary>
        /// The unique identifier for this formation, such as 6-2
        /// </summary>
        public string id;
        
        [Header("Text")]
        [InfoBox("Such as a 6-2 Formation for Volleyball"), HideInTables]
        public string title;

        [TextArea(5, 7), HideInTables]
        public string desc;
        */

        public Formation formation;

        // corresponding to rotation Numbers 1 through 6
        [Range(1, 6)]
        public int rotationNumber;

        // The Situation that this Position Describes
        //[EnumToggleButtons]
        public Situation situation;

        public override string ToString()
        {
            return $"{formation} rotNum[{rotationNumber}] {situation}";
        }

        [HideInTables]
        public Vector3[] positions = new Vector3[6];

        // The placeholders for positions of the players in this rotation's situation.
        // This is indexed by the position index, 0 through 5, corresponding to players 1 through 6 in this rotation, where index 0 represents player 1, who is in the back right of rotation index 0.
        [HideInTables]
        public List<GameObject> positionMarkers = new List<GameObject>();

        // The prefab that will be instantiated for each position marker, to visualize the position.
        [HideInTables]
        public GameObject positionMarkerPrefab;

        [HideInTables]
        public bool allowPositionOverwrite = false;
        [HideInTables]
        public bool allowPositionMarkerOverwrite = false;


        [HideInTables, Button("UseExistingPlayerMarkers", Name = "Use Existing Player Markers"), ButtonGroup("Visualize"), GUIColor("green")]
        private void UseExistingPlayerMarkers()
        {
            positionMarkers.Clear();

            var gos = GameObject.FindObjectsOfType<PlayerMarker>();

            // Sort by playerNumber
            gos.Sort((a, b) => a.playerNumber.CompareTo(b.playerNumber));

            foreach ( var marker in gos)
            {
                  positionMarkers.Add(marker.gameObject);
            }

            // If they're not already populated, find the PlayerMarkers and put them in the positionMarkers list
            if (positionMarkers.Count == 0)
            {
                // Find the PlayerMarkers
                GameObject[] playerMarkers = GameObject.FindGameObjectsWithTag("PlayerMarker");

                // Populate the positionMarkers list with the playerMarkers
                foreach (GameObject playerMarker in playerMarkers)
                {
                    positionMarkers.Add(playerMarker);
                }
            }
        }

        [HideInTables]
        public bool enableDestroyDuringCleanUp = false;

        [HideInTables, Button("CleanUp",Name ="Clean Up"), ButtonGroup("CleanUp"),GUIColor("red")]
        private void CleanUp()
        {
            if (enableDestroyDuringCleanUp)
            { 
                // Destroy all PositionMarkers, then remove them from the list
                foreach (GameObject positionMarker in positionMarkers)
                {
                    DestroyImmediate(positionMarker);
                }
            }

            positionMarkers.Clear();
        }

        [HideInTables, Button("VisualizePosition",Name ="Visualize"), ButtonGroup("Visualize"), GUIColor("green")]
        private void VisualizePositions() 
        {
          
            // If they're not already populated, instantiate the Position Marker Prefabs, 6 total, and put them at the positions indicated (TransferPositionsToPositionMarkers)
            if (positionMarkers.Count == 0 && positionMarkerPrefab != null)
            {
                // Instantiate the position marker prefabs
                for (int i = 0; i < 6; i++)
                {
                    GameObject positionMarker = Instantiate(positionMarkerPrefab);
                    positionMarker.name = "Position Marker " + i;
                    positionMarkers.Add(positionMarker);
                }
            }
            
            // Save the current value of allowPositionOverwrite
            bool tempAllowPositionOverwrite = allowPositionOverwrite;
            
            // Temporarily ensure that allowPositionOverwrite is true, so that the positions can be overwritten to match the position markers.
            allowPositionMarkerOverwrite = true;

            TransferPositionsToPositionMarkers();

            // Restore the allowPositionOverwrite to its original value
            allowPositionMarkerOverwrite = tempAllowPositionOverwrite;
        }

        [HideInTables]
        [Button("Transfer Position Markers to Positions"), ButtonGroup("Transfer"), GUIColor("green")]
        private void TransferPositionMarkersToPositions()
        {
            if (!allowPositionOverwrite)
            {
                Debug.LogWarning("Position Overwrite is not allowed in " + this.name+ ", so no change to position.");
                return;
            }

            for (int i = 0; i < positionMarkers.Count; i++)
            {
                if (positionMarkers[i] == null)
                {
                    Debug.LogWarning("Position Marker " + i + " is null in " + this.name + ", so no change to position.");
                    continue;
                }
                // Transfer the positionMarkers x y and z to the positions array
                positions[i].x = positionMarkers[i].transform.position.x;
                positions[i].y = positionMarkers[i].transform.position.y;
                positions[i].z = positionMarkers[i].transform.position.z;
            }
        }

        [HideInTables]
        [Button("Transfer Positions to Position Markers"), ButtonGroup("Transfer")]
        private void TransferPositionsToPositionMarkers()
        {
            if(!allowPositionMarkerOverwrite)
            {
                Debug.LogWarning("Position Overwrite is not allowed in " + this.name + ", so no change to position.");
                return;
            }

            for (int i = 0; i < positionMarkers.Count; i++)
            {
                if (positionMarkers[i] == null)
                {
                    Debug.LogWarning("Position Marker " + i + " is null in " + this.name + ", so no change to position.");
                    continue;
                }
                // Transfer the positionMarkers x y and z to the positions array
                positionMarkers[i].transform.position = positions[i];
            }
        }

        [HideInTables]
        [Button("Empty Position Marker References"), ButtonGroup("CleanUp")]
        private void EmptyPositionMarkerReferences()
        {
            for (int i = 0; i < positionMarkers.Count; i++)
            {
                positionMarkers.Clear();
            }
        }

        public static List<DataPosition> data_list = new List<DataPosition>();

        public static void Load(string folder = "")
        {
            if (data_list.Count == 0)
                data_list.AddRange(Resources.LoadAll<DataPosition>(folder));
        }


    }

}
