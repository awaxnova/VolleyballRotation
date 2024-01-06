using Arrow;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VolleyballRotation;

namespace VolleyballRotation
{

    [System.Serializable]
    public class FormationData
    {
        public Formation formation;
        public List<RotationData> rotationData;

        public FormationData(Formation currentFormation)
        {
            formation = currentFormation;

            rotationData = new List<RotationData>();

            if (DataPosition.data_list.Count == 0)
                DataPosition.Load();

            // Find the DataPositions that correspond to this Formation
            List<DataPosition> baseDataPositions = DataPosition.data_list.FindAll(x => x.formation == currentFormation);

            // Assert that we found at least one DataPosition
            if (baseDataPositions.Count == 0)
            {
                Debug.LogError("No DataPositions found for Formation " + currentFormation);
                return;
            }

            // Pull the baseDataPositions into a data structure,
            for (int i = 0; i < baseDataPositions.Count; i++)
            {
                // For each DataPosition, extract the Situation, RotationNumber, Positions, PlayerNames, and PlayerNamesOverrides
                DataPosition dpos = baseDataPositions[i];

                var defRot = new RotationData(dpos);
                var loadedRot = defRot.Load();

                // Capture the dpos into a RotationData, and add it to the list.
                if(loadedRot != null)
                {
                    // Override by Loading PlayerPrefs
                    rotationData.Add(loadedRot);
                }
                else
                {
                    // Default values
                    rotationData.Add(defRot);
                }

                // TODO - Override by Player Settings
            }
        }

        // Save an entire formation to PlayerPrefs.
        public void Save()
        {
            for (int i = 0; i < rotationData.Count; i++)
            {
                rotationData[i].Save();
            }
        }

        public RotationData GetRotationData(Situation situation, int rotationNumber)
        {
            return rotationData.Find(x => x.situation == situation && x.rotationNumber == rotationNumber);
        }

        public Vector3 GetPosition(Situation situation, int rotationNumber, int positionNumber)
        {
            RotationData rotData = GetRotationData(situation, rotationNumber);
            if(rotData == null)
            {
                Debug.LogError("No RotationData found for Situation " + situation + " and RotationNumber " + rotationNumber);
                return Vector3.zero;
            }
            if(rotData.positions.Count <= positionNumber)
            {
                Debug.LogError("No Position found for Situation " + situation + " and RotationNumber " + rotationNumber + " and PositionNumber " + positionNumber);
                return Vector3.zero;
            }   

            return rotData.positions[positionNumber];
        }

        public string GetPlayerName(Situation situation, int rotationNumber, int positionNumber)
        {
            RotationData rotData = GetRotationData(situation, rotationNumber);
            if (rotData == null)
            {
                Debug.LogError("No RotationData found for Situation " + situation + " and RotationNumber " + rotationNumber);
                return "";
            }
            if (rotData.playerNames.Count <= positionNumber)
            {
                Debug.LogError("No PlayerName found for Situation " + situation + " and RotationNumber " + rotationNumber + " and PositionNumber " + positionNumber);
                return "";
            }
            return rotData.playerNames[positionNumber];
        }

        public AnimatedArrowRenderer.ArrowTypes GetArrowType(Situation situation, int rotationNumber, int positionNumber)
        {
            RotationData rotData = GetRotationData(situation, rotationNumber);
            if (rotData == null)
            {
                Debug.LogError("No RotationData found for Situation " + situation + " and RotationNumber " + rotationNumber);
                return AnimatedArrowRenderer.ArrowTypes.None;
            }
            if (rotData.arrowTypes.Count <= positionNumber)
            {
                Debug.LogError("No ArrowType found for Situation " + situation + " and RotationNumber " + rotationNumber + " and PositionNumber " + positionNumber);
                return AnimatedArrowRenderer.ArrowTypes.None;
            }
            return rotData.arrowTypes[positionNumber];
        }

        public AnimatedArrowRenderer.SegmentTypes GetSegmentType(Situation situation, int rotationNumber, int positionNumber)
        {
            RotationData rotData = GetRotationData(situation, rotationNumber);

            if (rotData == null)
            {
                Debug.LogError("No RotationData found for Situation " + situation + " and RotationNumber " + rotationNumber);
                return AnimatedArrowRenderer.SegmentTypes.None;
            }
            if (rotData.segmentTypes.Count <= positionNumber)
            {
                Debug.LogError("No SegmentType found for Situation " + situation + " and RotationNumber " + rotationNumber + " and PositionNumber " + positionNumber);
                return AnimatedArrowRenderer.SegmentTypes.None;
            }
            return rotData.segmentTypes[positionNumber];
        }

        public float GetArrowHeight(Situation situation, int rotationNumber, int positionNumber)
        {
            RotationData rotData = GetRotationData(situation, rotationNumber);

            if (rotData == null)
            {
                Debug.LogError("No RotationData found for Situation " + situation + " and RotationNumber " + rotationNumber);
                return 0f;
            }
            if (rotData.arrowHeights.Count <= positionNumber)
            {
                Debug.LogError("No ArrowHeight found for Situation " + situation + " and RotationNumber " + rotationNumber + " and PositionNumber " + positionNumber);
                return 0f;
            }
            return rotData.arrowHeights[positionNumber];
        }

        public float GetArrowSegmentLength(Situation situation, int rotationNumber, int positionNumber)
        {
            RotationData rotData = GetRotationData(situation, rotationNumber);
            if (rotData == null)
            {
                Debug.LogError("No RotationData found for Situation " + situation + " and RotationNumber " + rotationNumber);
                return 0f;
            }
            if (rotData.arrowSegmentLengths.Count <= positionNumber)
            {
                Debug.LogError("No ArrowSegmentLength found for Situation " + situation + " and RotationNumber " + rotationNumber + " and PositionNumber " + positionNumber);
                return 0f;
            }
            return rotData.arrowSegmentLengths[positionNumber];
        }

        public override string ToString()
        {
            string s = "Formation: " + formation + "\n";
            for (int i = 0; i < rotationData.Count; i++)
            {
                s += "\tRotation " + rotationData[i].rotationNumber + " Situation:" + rotationData[i].situation +  "\n";
                for (int j = 0; j < rotationData[i].positions.Count; j++)
                {
                    s += "\t\tPosition " + j + ": " + rotationData[i].playerNames[j] + " pos:" + rotationData[i].positions[j] + "\n";
                }
            }
            return s;
        }
    }

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

        // Save RotationData to PlayerPrefs with a custom key
        private void Save(string key)
        {
            // Convert RotationData to JSON string
            string json = JsonUtility.ToJson(this);

            // Save JSON string to PlayerPrefs
            PlayerPrefs.SetString(key, json);
            PlayerPrefs.Save();
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
            return rotationData;
        }

        // Generate a custom key based on Formation, Situation, and RotationNumber
        private string GenerateKey()
        {
            return $"{formation}_{situation}_{rotationNumber}";
        }
    }
}