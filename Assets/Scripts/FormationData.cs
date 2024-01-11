using Arrow;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VolleyballRotation
{

    [System.Serializable]
    public class FormationData
    {
        public Formation formation;

        public Color frontCourtColor = Color.gray;
        public Color backCourtColor = Color.black;

        [NonSerialized]
        public List<RotationData> rotationData;

        [NonSerialized]
        string jsonSnapshot;

        public FormationData(Formation currentFormation, bool loadPlayerPrefs = true)
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

                RotationData loadedRot = null;

                if (loadPlayerPrefs)
                {
                    loadedRot = defRot.Load();
                }

                MergeRotationDatas(loadedRot, defRot);

            }

            // Populate the FormationData from a snapshot
            if(loadPlayerPrefs)
                PopulateFromSnapshot();
        }

        private string GenerateKey()
        {
            return $"{formation}";
        }

        private void Save(string key)
        {
            Debug.Log($"FormationData.Save {GenerateKey()}");

            // Convert to JSON string
            string json = JsonUtility.ToJson(this);

            // Save JSON string to PlayerPrefs
            PlayerPrefs.SetString(key, json);
            PlayerPrefs.Save();
            SnapshotFormationData();
        }

        // Load RotationData from PlayerPrefs with a custom key
        private FormationData Load(string key)
        {
            Debug.Log($"FormationData.Load {GenerateKey()}");
            // Load JSON string from PlayerPrefs
            string json = PlayerPrefs.GetString(key);

            // If the key is not found or the JSON string is empty, return null
            if (string.IsNullOrEmpty(json))
            {
                return null;
            }

            // Convert JSON string to FormationData object
            FormationData formationData = JsonUtility.FromJson<FormationData>(json);
            formationData.SnapshotFormationData();
            return formationData;
        }

        private void PopulateFromSnapshot()
        {
            Debug.Log($"RotationData.PopulateFromSnapshot {GenerateKey()}");

            if(!String.IsNullOrEmpty(jsonSnapshot))
                JsonUtility.FromJsonOverwrite(jsonSnapshot, this);
        }

        protected string SnapshotFormationData()
        {
            Debug.Log($"FormationData.SnapshotFormationData {GenerateKey()}");

            jsonSnapshot = JsonUtility.ToJson(this);
            return jsonSnapshot;
        }

        private void MergeRotationDatas(RotationData loadedRot, RotationData defRot)
        {

            // Capture the dpos into a RotationData, and add it to the list.
            if (loadedRot != null)
            {

                // Let's choose which RotationData to use... if the loadedRot has an empty list,
                // then take the list from the defRot, and save it into PlayerPrefs.
                // This is to allow for new members to be added to the RotationData class, and still be backwards compatible.
                bool dirty = false;
                // Iterate over all lists in the loadedRot,
                if (loadedRot.positions.Count == 0)
                {
                    // If the loadedRot has an empty list, then take the list from the defRot
                    loadedRot.positions = defRot.positions;
                    dirty = true;
                }
                if (loadedRot.playerNames.Count == 0)
                {
                    // If the loadedRot has an empty list, then take the list from the defRot
                    loadedRot.playerNames = defRot.playerNames;
                    dirty = true;
                }
                if (loadedRot.arrowTypes.Count == 0)
                {
                    // If the loadedRot has an empty list, then take the list from the defRot
                    loadedRot.arrowTypes = defRot.arrowTypes;
                    dirty = true;
                }
                if (loadedRot.segmentTypes.Count == 0)
                {
                    // If the loadedRot has an empty list, then take the list from the defRot
                    loadedRot.segmentTypes = defRot.segmentTypes;
                    dirty = true;
                }
                if (loadedRot.arrowHeadColors.Count == 0)
                {
                    // If the loadedRot has an empty list, then take the list from the defRot
                    loadedRot.arrowHeadColors = defRot.arrowHeadColors;
                    dirty = true;
                }
                if (loadedRot.arrowSegmentColors.Count == 0)
                {
                    // If the loadedRot has an empty list, then take the list from the defRot
                    loadedRot.arrowSegmentColors = defRot.arrowSegmentColors;
                    dirty = true;
                }
                if (loadedRot.arrowHeights.Count == 0)
                {
                    // If the loadedRot has an empty list, then take the list from the defRot
                    loadedRot.arrowHeights = defRot.arrowHeights;
                    dirty = true;
                }
                if (loadedRot.arrowSegmentLengths.Count == 0)
                {
                    // If the loadedRot has an empty list, then take the list from the defRot
                    loadedRot.arrowSegmentLengths = defRot.arrowSegmentLengths;
                    dirty = true;
                }

                // If the defRot has an empty list, and Player Prefs doesn't, then empty the PlayerPrefs by saving over it.
                // Prevents bloat in the player prefs if certain members are removed.
                if(defRot.positions.Count == 0 && loadedRot.positions.Count > 0)
                {
                    loadedRot.positions.Clear();
                    dirty = true;
                }
                if (defRot.playerNames.Count == 0 && loadedRot.playerNames.Count > 0)
                {
                    loadedRot.playerNames.Clear();
                    dirty = true;
                }
                if (defRot.arrowTypes.Count == 0 && loadedRot.arrowTypes.Count > 0)
                {
                    loadedRot.arrowTypes.Clear();
                    dirty = true;
                }
                if (defRot.segmentTypes.Count == 0 && loadedRot.segmentTypes.Count > 0)
                {
                    loadedRot.segmentTypes.Clear();
                    dirty = true;
                }
                if (defRot.arrowHeadColors.Count == 0 && loadedRot.arrowHeadColors.Count > 0)
                {
                    loadedRot.arrowHeadColors.Clear();
                    dirty = true;
                }
                if (defRot.arrowSegmentColors.Count == 0 && loadedRot.arrowSegmentColors.Count > 0)
                {
                    loadedRot.arrowSegmentColors.Clear();
                    dirty = true;
                }
                if (defRot.arrowHeights.Count == 0 && loadedRot.arrowHeights.Count > 0)
                {
                    loadedRot.arrowHeights.Clear();
                    dirty = true;
                }
                if (defRot.arrowSegmentLengths.Count == 0 && loadedRot.arrowSegmentLengths.Count > 0)
                {
                    loadedRot.arrowSegmentLengths.Clear();
                    dirty = true;
                }


                if (dirty)
                    loadedRot.Save();

                // Override by Loading PlayerPrefs
                rotationData.Add(loadedRot);
            }
            else
            {
                // Default values
                rotationData.Add(defRot);
            }
        }

        /// <summary>
        /// Save an entire formation to PlayerPrefs.
        /// </summary>
        /// <param name="forceSave"></param>
        public void Save(bool forceSave = false)
        {
            for (int i = 0; i < rotationData.Count; i++)
            {
                if(rotationData[i].isDifferentFromSnapshot() || forceSave)
                    rotationData[i].Save();
            }

            Save(GenerateKey());
        }

        public void Snapshot()
        {
            SnapshotFormationData();

            for (int i = 0; i < rotationData.Count; i++)
            {
                rotationData[i].SnapshotRotationData();
            }
        }

        public void Revert()
        {
            for (int i = 0; i < rotationData.Count; i++)
            {
                if (rotationData[i].isDifferentFromSnapshot())
                    rotationData[i].PopulateFromSnapshot();
            }

            PopulateFromSnapshot();
        }

        

        public RotationData GetRotationData(Situation situation, int rotationNumber)
        {
            RotationData rotData = rotationData.Find(x => (x.situation == situation) && (x.rotationNumber == rotationNumber));

            if (rotData == null)
            {
                Debug.LogError("No RotationData found for Situation " + situation + " and RotationNumber " + rotationNumber);
                return null;
            }

            return rotData;
        }

        #region GettersCompact
        // Define a generic method to get a value from a list based on certain criteria
        private T GetValue<T>(Situation situation, int rotationNumber, int positionNumber, List<T> dataList, string errorLog)
        {
            if (dataList != null && dataList.Count < positionNumber)
            {
                string dataListString = "";
                foreach (var item in dataList)
                {
                    dataListString += item.ToString() + ", ";
                }
                
                Debug.LogError("No data found for " + errorLog + " Count:" + dataList.Count + "\n" + dataListString + "\n" + this.ToString());
                return default(T);
            }
            return dataList[positionNumber - 1];
        }

        // Define a generic method to set a value in a list based on certain criteria
        private void SetValue<T>(Situation situation, int rotationNumber, int positionNumber, List<T> dataList, T value, string errorLog)
        {
            if (dataList != null && dataList.Count < positionNumber)
            {
                string dataListString = "";
                foreach (var item in dataList)
                {
                    dataListString += item.ToString() + ", ";
                }
                Debug.LogError("No data found for " + errorLog + " Count:"+ dataList.Count + "\n" + dataListString + "\n" + this.ToString());
                return;
            }
            dataList[positionNumber - 1] = value;
        }
        // Public methods for getting and setting positions
        public Vector3 GetPosition(Situation situation, int rotationNumber, int positionNumber)
        {
            return GetValue(situation, rotationNumber, positionNumber, GetRotationData(situation, rotationNumber).positions, 
                               $"Situation {situation}, RotationNumber {rotationNumber}, PositionNumber {positionNumber}");
        }

        public void SetPosition(Situation situation, int rotationNumber, int positionNumber, Vector3 value)
        {
            SetValue(situation, rotationNumber, positionNumber, GetRotationData(situation, rotationNumber).positions, value, 
                               $"Situation {situation}, RotationNumber {rotationNumber}, PositionNumber {positionNumber}");
        }

        // Public methods for getting and setting player names
        public string GetPlayerName(Situation situation, int rotationNumber, int positionNumber)
        {
            return GetValue(situation, rotationNumber, positionNumber, GetRotationData(situation, rotationNumber).playerNames, 
                               $"Situation {situation}, RotationNumber {rotationNumber}, PositionNumber {positionNumber}");
        }

        public void SetPlayerName(Situation situation, int rotationNumber, int positionNumber, string value)
        {
            SetValue(situation, rotationNumber, positionNumber, GetRotationData(situation, rotationNumber).playerNames, value, 
                               $"Situation {situation}, RotationNumber {rotationNumber}, PositionNumber {positionNumber}");
        }

        // Public methods for getting and setting arrow types

        public AnimatedArrowRenderer.ArrowTypes GetArrowType(Situation situation, int rotationNumber, int positionNumber)
        {
            return GetValue(situation, rotationNumber, positionNumber, GetRotationData(situation, rotationNumber).arrowTypes, 
                               $"Situation {situation}, RotationNumber {rotationNumber}, PositionNumber {positionNumber}");
        }

        public void SetArrowType(Situation situation, int rotationNumber, int positionNumber, AnimatedArrowRenderer.ArrowTypes value)
        {
            SetValue(situation, rotationNumber, positionNumber, GetRotationData(situation, rotationNumber).arrowTypes, value, 
                               $"Situation {situation}, RotationNumber {rotationNumber}, PositionNumber {positionNumber}");
        }

        // Public methods for getting and setting segment types

        public AnimatedArrowRenderer.SegmentTypes GetSegmentType(Situation situation, int rotationNumber, int positionNumber)
        {
            return GetValue(situation, rotationNumber, positionNumber, GetRotationData(situation, rotationNumber).segmentTypes, 
                            $"Situation {situation}, RotationNumber {rotationNumber}, PositionNumber {positionNumber}");
        }

        public void SetSegmentType(Situation situation, int rotationNumber, int positionNumber, AnimatedArrowRenderer.SegmentTypes value)
        {
            SetValue(situation, rotationNumber, positionNumber, GetRotationData(situation, rotationNumber).segmentTypes, value, 
                            $"Situation {situation}, RotationNumber {rotationNumber}, PositionNumber {positionNumber}");
        }

        // Public methods for getting and setting arrow head colors
        public Color GetArrowHeadColor(Situation situation, int rotationNumber, int positionNumber)
        {
            return GetValue(situation, rotationNumber, positionNumber, GetRotationData(situation, rotationNumber).arrowHeadColors, 
                            $"Situation {situation}, RotationNumber {rotationNumber}, PositionNumber {positionNumber}");
        }

        public void SetArrowHeadColor(Situation situation, int rotationNumber, int positionNumber, Color value)
        {
            SetValue(situation, rotationNumber, positionNumber, GetRotationData(situation, rotationNumber).arrowHeadColors, value, 
                            $"Situation {situation}, RotationNumber {rotationNumber}, PositionNumber {positionNumber}");
        }

        // Public methods for getting and setting arrow segment colors
        public Color GetArrowSegmentColor(Situation situation, int rotationNumber, int positionNumber)
        {
            return GetValue(situation, rotationNumber, positionNumber, GetRotationData(situation, rotationNumber).arrowSegmentColors, 
                            $"Situation {situation}, RotationNumber {rotationNumber}, PositionNumber {positionNumber}");
        }


        public void SetArrowSegmentColor(Situation situation, int rotationNumber, int positionNumber, Color value)
        {
            SetValue(situation, rotationNumber, positionNumber, GetRotationData(situation, rotationNumber).arrowSegmentColors, value, 
                            $"Situation {situation}, RotationNumber {rotationNumber}, PositionNumber {positionNumber}");
        }

        // Public methods for getting and setting arrow heights
        public float GetArrowHeight(Situation situation, int rotationNumber, int positionNumber)
        {
            return GetValue(situation, rotationNumber, positionNumber, GetRotationData(situation, rotationNumber).arrowHeights, 
                            $"Situation {situation}, RotationNumber {rotationNumber}, PositionNumber {positionNumber}");
        }

        public void SetArrowHeight(Situation situation, int rotationNumber, int positionNumber, float value)
        {
            SetValue(situation, rotationNumber, positionNumber, GetRotationData(situation, rotationNumber).arrowHeights, value, 
                            $"Situation {situation}, RotationNumber {rotationNumber}, PositionNumber {positionNumber}");
        }

        // Public methods for getting and setting arrow segment lengths
        public float GetArrowSegmentLength(Situation situation, int rotationNumber, int positionNumber)
        {
            return GetValue(situation, rotationNumber, positionNumber, GetRotationData(situation, rotationNumber).arrowSegmentLengths, 
                            $"Situation {situation}, RotationNumber {rotationNumber}, PositionNumber {positionNumber}");
        }

        public void SetArrowSegmentLength(Situation situation, int rotationNumber, int positionNumber, float value)
        {
            SetValue(situation, rotationNumber, positionNumber, GetRotationData(situation, rotationNumber).arrowSegmentLengths, value, 
                            $"Situation {situation}, RotationNumber {rotationNumber}, PositionNumber {positionNumber}");
        }
        #endregion


        #region Getters


        #endregion

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

}