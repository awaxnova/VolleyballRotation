using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Arrow;

namespace VolleyballRotation
{

    [System.Serializable]
    public class RotationData
    {
        public Formation formation;
        public Situation situation;
        public int rotationNumber;
        public List<string> playerNames;

        public List<Vector3> positions;

        public List<AnimatedArrowRenderer.ArrowTypes> arrowTypes;
        public List<AnimatedArrowRenderer.SegmentTypes> segmentTypes;

        public List<float> arrowHeights;
        public List<float> arrowSegmentLengths;

        [System.NonSerialized]
        private string jsonSnapshot;

        public RotationData(DataPosition dpos)
        {
            // Capture the dpos situation, rotationNumber, playernames, and positions
            formation = dpos.formation;

            situation = dpos.situation;

            rotationNumber = dpos.rotationNumber;

            playerNames = new List<string>();
            for (int i = 0; i < dpos.playerNamesOverrides.Length; i++)
            {
                playerNames.Add(dpos.playerNamesOverrides[i]);
            }

            positions = new List<Vector3>();
            for (int i = 0; i < dpos.positions.Length; i++)
            {
                positions.Add(dpos.positions[i]);
            }

            arrowTypes = new List<AnimatedArrowRenderer.ArrowTypes>();
            for (int i = 0; i < dpos.arrowTypes.Count; i++)
            {
                arrowTypes.Add(dpos.arrowTypes[i]);
            }

            segmentTypes = new List<AnimatedArrowRenderer.SegmentTypes>();
            for (int i = 0; i < dpos.segmentTypes.Count; i++)
            {
                segmentTypes.Add(dpos.segmentTypes[i]);
            }

            arrowHeights = new List<float>();
            for (int i = 0; i < dpos.arrowHeights.Count; i++)
            {
                arrowHeights.Add(dpos.arrowHeights[i]);
            }

            arrowSegmentLengths = new List<float>();
            for (int i = 0; i < dpos.arrowSegmentLengths.Count; i++)
            {
                arrowSegmentLengths.Add(dpos.arrowSegmentLengths[i]);
            }

            // Upon creation, capture a snapshot of this RotationData
            SnapshotRotationData();
        }

        // Save RotationData to PlayerPrefs
        public void Save()
        {
            string key = GenerateKey();
            Save(key);
        }

        // Load RotationData from PlayerPrefs
        public RotationData Load()
        {
            string key = GenerateKey();
            return Load(key);
        }

        /// <summary>
        /// Capture a snapshot of this RotationData
        /// </summary>
        /// <returns></returns>
        public string SnapshotRotationData() { 
            jsonSnapshot = JsonUtility.ToJson(this);
            return jsonSnapshot;
        }

        /// <summary>
        /// Compare RotationData to a snapshot
        /// </summary>
        /// <returns></returns>
        public bool isDifferentFromSnapshot()
        {
            string json = JsonUtility.ToJson(this);
            return json != jsonSnapshot;
        }

        /// <summary>
        /// Overwrite this RotationData with the values from a snapshot
        /// </summary>
        /// <param name="json"></param>
        public void PopulateFromSnapshot()
        {
            JsonUtility.FromJsonOverwrite(jsonSnapshot, this);
        }

        // Save RotationData to PlayerPrefs with a custom key
        private void Save(string key)
        {
            // Convert RotationData to JSON string
            string json = JsonUtility.ToJson(this);

            // Save JSON string to PlayerPrefs
            PlayerPrefs.SetString(key, json);
            PlayerPrefs.Save();

            // Set a new snapshot to represent what's saved.
            SnapshotRotationData();
        }

        // Load RotationData from PlayerPrefs with a custom key
        private RotationData Load(string key)
        {
            // Load JSON string from PlayerPrefs
            string json = PlayerPrefs.GetString(key);

            // If the key is not found or the JSON string is empty, return null
            if (string.IsNullOrEmpty(json))
            {
                return null;
            }

            // Convert JSON string to RotationData object
            RotationData rotationData = JsonUtility.FromJson<RotationData>(json);            
            rotationData.SnapshotRotationData();
            return rotationData;
        }

        // Generate a custom key based on Formation, Situation, and RotationNumber
        private string GenerateKey()
        {
            return $"{formation}_{situation}_{rotationNumber}";
        }
    }
}
