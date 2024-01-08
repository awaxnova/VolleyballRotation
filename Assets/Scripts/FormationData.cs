using Arrow;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

        #region Getters
        public Vector3 GetPosition(Situation situation, int rotationNumber, int positionNumber)
        {
            RotationData rotData = GetRotationData(situation, rotationNumber);
            if(rotData == null)
            {
                Debug.LogError("No RotationData found for Situation " + situation + " and RotationNumber " + rotationNumber);
                return Vector3.zero;
            }
            if(rotData.positions.Count < positionNumber)
            {
                Debug.LogError("No Position found for Situation " + situation + " and RotationNumber " + rotationNumber + " and PositionNumber " + positionNumber);
                return Vector3.zero;
            }   

            return rotData.positions[positionNumber-1];
        }

        public string GetPlayerName(Situation situation, int rotationNumber, int positionNumber)
        {
            RotationData rotData = GetRotationData(situation, rotationNumber);
            if (rotData == null)
            {
                Debug.LogError("No RotationData found for Situation " + situation + " and RotationNumber " + rotationNumber);
                return "";
            }
            if (rotData.playerNames.Count < positionNumber)
            {
                Debug.LogError("No PlayerName found for Situation " + situation + " and RotationNumber " + rotationNumber + " and PositionNumber " + positionNumber);
                return "";
            }
            return rotData.playerNames[positionNumber-1];
        }

        public AnimatedArrowRenderer.ArrowTypes GetArrowType(Situation situation, int rotationNumber, int positionNumber)
        {
            RotationData rotData = GetRotationData(situation, rotationNumber);
            if (rotData == null)
            {
                Debug.LogError("No RotationData found for Situation " + situation + " and RotationNumber " + rotationNumber);
                return AnimatedArrowRenderer.ArrowTypes.RedPoint;
            }
            if (rotData.arrowTypes.Count < positionNumber)
            {
                Debug.LogError("No ArrowType found for Situation " + situation + " and RotationNumber " + rotationNumber + " and PositionNumber " + positionNumber);
                return AnimatedArrowRenderer.ArrowTypes.RedPoint;
            }
            return rotData.arrowTypes[positionNumber - 1];
        }

        public AnimatedArrowRenderer.SegmentTypes GetSegmentType(Situation situation, int rotationNumber, int positionNumber)
        {
            RotationData rotData = GetRotationData(situation, rotationNumber);

            if (rotData == null)
            {
                Debug.LogError("No RotationData found for Situation " + situation + " and RotationNumber " + rotationNumber);
                return AnimatedArrowRenderer.SegmentTypes.BlueZip;
            }
            if (rotData.segmentTypes.Count < positionNumber)
            {
                Debug.LogError("No SegmentType found for Situation " + situation + " and RotationNumber " + rotationNumber + " and PositionNumber " + positionNumber);
                return AnimatedArrowRenderer.SegmentTypes.BlueZip;
            }
            return rotData.segmentTypes[positionNumber - 1];
        }

        public float GetArrowHeight(Situation situation, int rotationNumber, int positionNumber)
        {
            RotationData rotData = GetRotationData(situation, rotationNumber);

            if (rotData == null)
            {
                Debug.LogError("No RotationData found for Situation " + situation + " and RotationNumber " + rotationNumber);
                return 1.5f;
            }
            if (rotData.arrowHeights.Count < positionNumber)
            {
                Debug.LogError("No ArrowHeight found for Situation " + situation + " and RotationNumber " + rotationNumber + " and PositionNumber " + positionNumber + " out of: " + rotData.arrowHeights.Count);
                return 1.5f;
            }
            //Debug.Log($"DIAGNOSTIC: GetArrowHeight() {situation}:{rotationNumber}:{positionNumber} qtyArrowHeights[]={rotData.arrowHeights.Count}");
            return rotData.arrowHeights[positionNumber - 1];
        }

        public float GetArrowSegmentLength(Situation situation, int rotationNumber, int positionNumber)
        {
            RotationData rotData = GetRotationData(situation, rotationNumber);
            if (rotData == null)
            {
                Debug.LogError("No RotationData found for Situation " + situation + " and RotationNumber " + rotationNumber);
                return 1.5f;
            }
            if (rotData.arrowSegmentLengths.Count < positionNumber)
            {
                Debug.LogError("No ArrowSegmentLength found for Situation " + situation + " and RotationNumber " + rotationNumber + " and PositionNumber " + positionNumber);
                return 1.5f;
            }
            return rotData.arrowSegmentLengths[positionNumber - 1];
        }
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