using Arrow;
using DG.Tweening;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace VolleyballRotation
{
    public class RotationManager : MonoBehaviour
    {

        [Header("Rotation Markers - show the player rotation numeric names")]
        public GameObject[] rotationMarkers = new GameObject[6];

        [Header("The Prefab to use as the player Marker")]
        public GameObject playerMarkerPrefab;

        [Header("Player Markers - show the player position names")]
        private List<GameObject> playerMarkers = new List<GameObject>();

        public string[] playerNames = new string[6];

        // Capture the initial positions of the Rotation Markers, so we can save where each rotation position belongs.
        private GameObject[] initialPositions = new GameObject[6];

        /// <summary>
        /// The prefabs used to indicate the numeric digits to display for the current rotation.
        /// </summary>
        public GameObject[] decimalPrefabs = new GameObject[10];

        /// <summary>
        /// The marker of the location where we want to display the current rotation's numeric representation.
        /// </summary>
        public GameObject currentRotationPosition;

        /// <summary>
        /// Control whether or not the Rotation Markers are shown.
        /// </summary>
        public bool showRotationMarkers = true;

        /// <summary>
        /// Control whether or not the Player Markers are shown.
        /// </summary>
        public bool showPlayerMarkers = true;

        /// <summary>
        /// How many second to take to transition the player positions to the new position.
        /// </summary>
        public float playerPositionTransitionDuration = 0.2f;

        public SuperTextMesh uiStatusDisplay;


        public UIButtonGroupColorControl buttonGroupRotation;
        public UIButtonGroupColorControl buttonGroupSituation;

        [Header("Current Position")]
        [ShowInInspector, Range(1,6)]
        [InlineButton("Rotate")]
        [InlineButton("BackRotate")]       
        public int currentRotation = 1;
        
        //[EnumToggleButtons,OnValueChanged("OnValueChangedCurrentSituation")]
        public Situation currentSituation = Situation.Rotation;

        [Header("Next Position")]
        [ShowInInspector, Range(1, 6)]
        [InlineButton("NextRotate","Rotate")]
        [InlineButton("NextBackRotate","BackRotate")]
        public int nextRotation = 1;
        //[EnumToggleButtons]
        public Situation nextSituation = Situation.Rotation;

        private int lastCurrentRotation = 0;
        private Situation lastCurrentSituation = Situation.None;

        private Formation lastFormation = Formation.None;
        private Formation currentFormation = Formation.F6_2;

        private FormationData currentFormationData;

        private void Rotate() { currentRotation = currentRotation == 6 ? 1 : Mathf.Clamp(currentRotation + 1, 1, 6); nextRotation = currentRotation; }
        private void BackRotate() { currentRotation = currentRotation == 1 ? 6 : Mathf.Clamp(currentRotation - 1, 1, 6); nextRotation = currentRotation; }
        private void NextRotate() { nextRotation = nextRotation == 6 ? 1 : Mathf.Clamp(nextRotation + 1, 1, 6); }
        private void NextBackRotate() { nextRotation = nextRotation == 1 ? 6 : Mathf.Clamp(nextRotation - 1, 1, 6); }

        // Start is called before the first frame update
        void Start()
        {
            DataRotation.Load();
            DataPosition.Load();

            // Copy each of the gameobjects from the playerPositions array to the initialPositions array
            for(int i = 0; i < rotationMarkers.Length; i++)
            {
                initialPositions[i] = new GameObject($"initialPos[{i}]");
                initialPositions[i].transform.position = rotationMarkers[i].transform.position;
                initialPositions[i].transform.rotation = rotationMarkers[i].transform.rotation;
                initialPositions[i].transform.localScale = rotationMarkers[i].transform.localScale;

            }

            for (int i = 0; i < 6; i++)
            {
                var pm = Instantiate(playerMarkerPrefab); 
                playerMarkers.Add(pm);               

                var stm = pm.GetComponentInChildren<SuperTextMesh>();
                if (stm != null)
                {
                    stm.text = playerNames[i];
                }

                var pmScript = pm.GetComponent<PlayerMarker>();
                if (pmScript != null)
                {
                    pmScript.playerNumber = i + 1;
                }
            }

        }

        // Update is called once per frame
        void Update()
        {
            if(lastFormation != currentFormation)
            {
                // The formation changed, so we need to update the player names and the player positions.
                UpdateNewFormation();
            }

            for (int i = 0; i < rotationMarkers.Length; i++)
            {
                rotationMarkers[i].SetActive(showRotationMarkers);
            }

            for (int i = 0; i < playerMarkers.Count; i++)
            {
                playerMarkers[i].SetActive(showPlayerMarkers);
            }

            // if the current rotation is different from the last rotation
            // then change the position of the players to match the initialPosition of the current rotation

            if(currentRotation != lastCurrentRotation)
            {
                for(int i = 0; i < rotationMarkers.Length; i++)
                {

                    // calculate the index of the next position, which should be the currentRotation minus the i value, but modulo 6.
                    int newPositionIndex = UpdatedPositionIndex(currentRotation, i);

                
                    Debug.Log($"[{i}]: newPositionIndex={newPositionIndex}, currentRotation={currentRotation}");

                    // I need to tween the position of the playerPositions[i] to the initialPositions[newPositionIndex]

                    // Move this GameObject to the targetTransform's position over 0.4 second
                    rotationMarkers[i].transform.DOMove(initialPositions[newPositionIndex].transform.position, playerPositionTransitionDuration).SetEase(Ease.Linear)
                        .OnComplete(() => { 
                            rotationMarkers[i].transform.rotation = initialPositions[newPositionIndex].transform.rotation;
                            rotationMarkers[i].transform.localScale = initialPositions[newPositionIndex].transform.localScale;
                        }
                        );

                }

                // Enable the decimalPrefabs[currentRotation] and disable the rest, at the location of the currentRotationPosition
                for(int j = 0; j < decimalPrefabs.Length; j++)
                {
                    if (decimalPrefabs[j] != null)
                    { 
                        decimalPrefabs[j].SetActive(j == currentRotation);
                        decimalPrefabs[j].transform.position = currentRotationPosition.transform.position;
                    }
                }

            }

            for(int i = 0; i < rotationMarkers.Length; i++)
            {
                if (rotationMarkers[i] != null)
                    FaceCamera(rotationMarkers[i]);
            }

            for(int i = 0; i < decimalPrefabs.Length; i++)
            {
                if (decimalPrefabs[i] != null)
                    FaceCamera(decimalPrefabs[i]);
            }

            UpdatePlayerPositions(currentRotation, currentSituation);
            UpdateNextPositionArrows(currentRotation, currentSituation, nextRotation, nextSituation);

            lastCurrentRotation = currentRotation;
            lastCurrentSituation = currentSituation;
            lastFormation = currentFormation;
        }

        private void UpdateNewFormation()
        {
            // Here, we choose the Data for the currentFormation, and pull the data into a data structure, which has parts that get overridden by playerprefs, and can be adjusted by in-game player settings.

            // TODO - Update the player names based on the currentFormation
            // TODO - Update the player positions based on the currentFormation

            
            currentFormationData = new FormationData(currentFormation);

            // FIXME - TODO - Override by Loading PlayerPrefs

            Debug.Log($"UpdateNewFormation() {currentFormationData}");
        }


        private void UpdatePlayerPositions(int currentRotation, Situation currentSituation)
        {
            if (lastCurrentRotation == currentRotation && lastCurrentSituation == currentSituation)
            {
                // No change in rotation or situation, so no need to update the player positions.
                return;
            }

            // Update the player positions based on the current rotation and situation

            // Find the DataRotation that matches the current rotation
            // Find the DataPosition that matches the current situation
            // Update the player positions based on the DataPosition

            RotationData rotationData = currentFormationData.GetRotationData(currentSituation, currentRotation);

            if (rotationData != null)
            {
                // Update the player positions based on the DataPosition
                for (int i = 0; i < playerMarkers.Count; i++)
                {
                    // Move this GameObject to the targetTransform's position over 0.4 second
                    playerMarkers[i].transform.DOMove(rotationData.positions[i], playerPositionTransitionDuration).SetEase(Ease.Linear)
                        .OnComplete(() =>
                        {
                            //playerMarkers[i].transform.rotation = dataPosition.positions[i].rotation;
                            //playerMarkers[i].transform.localScale = dataPosition.positions[i].localScale;
                        }
                    );

                    var stm = playerMarkers[i].GetComponentInChildren<SuperTextMesh>();
                    if (stm != null)
                    {
                        if(String.IsNullOrEmpty(rotationData.playerNames[i]))
                            stm.text = playerNames[i];
                        else                       
                            stm.text = rotationData.playerNames[i];
                    }

                }

            }
            else
            {


                string dataPositionString = DataPosition.data_list.Aggregate("", (current, next) => current + next.ToString() + "\n");

                Debug.LogWarning($"No DataPosition found for situation={currentSituation} and rotation={currentRotation} \n{dataPositionString}");
            }

            for(int i = 0; i < playerMarkers.Count; i++)
            {
                var stm = playerMarkers[i].GetComponentInChildren<SuperTextMesh>();
                FaceCamera(stm.gameObject,true);
            }
        }

        private void UpdateNextPositionArrows(int currentRotation, Situation currentSituation, int nextRotation, Situation nextSituation)
        {
            UpdateArrows();

            // If there is a difference in rotation or situation, then show the arrows, else hide them.
            if ((currentRotation != nextRotation) || (currentSituation != nextSituation))
            {
                // Show the arrows
                // TODO - Show the arrows
                // Get the Arrow Renderer from the PlayerMarker, and set the start to the current position, and the end to the next position.

                for(int playerIndex = 0; playerIndex < 6; playerIndex++)
                {
                    AnimatedArrowRenderer arrowRenderer = playerMarkers[playerIndex].GetComponentInChildren<AnimatedArrowRenderer>();

                    if(arrowRenderer != null)
                    {
                        Vector3 currPos = getCurrentPlayerPosition(playerIndex);
                        Vector3 nextPos = getNextPlayerPosition(playerIndex);

                        if ((currPos != null) && (nextPos != null) && (currPos != nextPos))
                        {
                            arrowRenderer.enabled = true;
                            arrowRenderer.TurnOnArrow();
                            arrowRenderer.SetPositions(currPos, nextPos);
                        }
                        else
                        {
                            //arrowRenderer.enabled = false;
                            arrowRenderer.TurnOffArrow();
                            //arrowRenderer.SetPositions(new Vector3(-1000f, -1000f, -1000f), new Vector3(-2000f, -2000f, -2000f));
                        }
                        
                    }
                    else
                    {
                        Debug.LogWarning($"No Arrow Renderer found for playerMarker[{playerIndex}]");
                    }

                }
            }
            else
            {
                // Hide the arrows
                // TODO - Hide the arrows
                for (int playerIndex = 0; playerIndex < 6; playerIndex++)
                {
                    AnimatedArrowRenderer arrowRenderer = playerMarkers[playerIndex].GetComponentInChildren<AnimatedArrowRenderer>();
                    if (arrowRenderer != null)
                    {
                        //arrowRenderer.enabled = false;
                        arrowRenderer.TurnOffArrow();
                    }
                }
            }

        }

        private Vector3 getNextPlayerPosition(int playerIndex)
        {
            RotationData rotationData = currentFormationData.GetRotationData(nextSituation, nextRotation);

            if (rotationData == null)
            {
                string dataPositionString = DataPosition.data_list.Aggregate("", (current, next) => current + next.ToString() + "\n");

                Debug.LogWarning($"No Current DataPosition found for situation={nextSituation} and rotation={nextRotation} \n{dataPositionString}");
                return new Vector3(-100f,-100f,-100f);
            }
            else
                return rotationData.positions[playerIndex];
        }

        private Vector3 getCurrentPlayerPosition(int playerIndex)
        {
            RotationData rotationData = currentFormationData.GetRotationData(currentSituation, currentRotation);

            if (rotationData == null)
            {
                string dataPositionString = DataPosition.data_list.Aggregate("", (current, next) => current + next.ToString() + "\n");
                Debug.LogWarning($"No Current DataPosition found for situation={currentSituation} and rotation={currentRotation} \n{dataPositionString}");
                return new Vector3(-200f,-200f,-200f);                
            }
            else
                return rotationData.positions[playerIndex];
        }

        private int UpdatedPositionIndex(int currentRotation, int playerIndex)
        {

            // ONE needs to go to where SIX is,
            // SIX needs to go to where FIVE is,
            // FIVE needs to go to where FOUR is,
            // FOUR needs to go to where THREE is,
            // THREE needs to go to where TWO is,
            // TWO needs to go to where ONE is

            // If we're in currentRotationIndex 0, then 1 should be where it initially was, and others should also be in their initial positions.
            // If we're in currentRotationIndex 1, then 1 should be where 6 was, and others should also go to where the one below them was.

            // Given the current rotation, and the index of the player, calculate the index of which initial position should be used.

            // Current rotation is constrained to be 1 through 6, but we want to change it into 0 through 5
            int currentRotationIndex = currentRotation - 1;
            switch(currentRotationIndex)
            {
                case 0:
                    // If we're in currentRotationIndex 0, then 1 should be where it initially was, and others should also be in their initial positions.
                    return playerIndex;
                case 1:
                    // If we're in currentRotationIndex 1, then 1 should be where 6 was, and others should also go to where the one below them was.
                    return (playerIndex + 5) % 6;
                case 2:
                    return (playerIndex + 4) % 6;
                case 3:
                    return (playerIndex + 3) % 6;
                case 4:
                    return (playerIndex + 2) % 6;
                case 5:
                    return (playerIndex + 1) % 6;
                default:
                    return 0;
            }

        }

        private void FaceCamera(GameObject objToTurn, bool negZAxisTowardCamera=false)
        {
            // Calculate the direction from this GameObject to the camera
            Vector3 lookDirection = Camera.main.transform.position - objToTurn.transform.position;

            if (negZAxisTowardCamera)
            { 
                // Face the negative Z-axis toward the camera
                lookDirection = -lookDirection;
            }

            // Adjust the rotation to look at the camera while keeping the Z-axis facing it
            objToTurn.transform.rotation = Quaternion.LookRotation(lookDirection, Vector3.up);
        }

        private void UpdateArrows()
        {
            for (int i = 0; i < playerMarkers.Count; i++)
            {
                AnimatedArrowRenderer arrowRenderer = playerMarkers[i].GetComponentInChildren<AnimatedArrowRenderer>();

                if (arrowRenderer != null)
                {
                    arrowRenderer.SetHeight(currentFormationData.GetArrowHeight(currentSituation, currentRotation, i)); 
                    arrowRenderer.SetSegmentLength(currentFormationData.GetArrowSegmentLength(currentSituation, currentRotation, i));
                    arrowRenderer.SetArrowHeadType(currentFormationData.GetArrowType(currentSituation, currentRotation, i));
                    arrowRenderer.SetArrowSegmentType(currentFormationData.GetSegmentType(currentSituation, currentRotation, i));
                }
                else
                {
                    Debug.LogWarning($"No Arrow Renderer found for playerMarker[{i}]");
                }

            }
        }

        #region Handlers
        public void ButtonRotation1()        { HandlerClickedRotation(1); }       
        public void ButtonRotation2()        { HandlerClickedRotation(2); }
        public void ButtonRotation3()        { HandlerClickedRotation(3); }
        public void ButtonRotation4()        { HandlerClickedRotation(4); }
        public void ButtonRotation5()        { HandlerClickedRotation(5); }
        public void ButtonRotation6()        { HandlerClickedRotation(6); }

        // Button handlers for Rotation, Serve, Base Defense, ServeReceive, and Attack
        public void ButtonRotation()        {HandlerClickedSituation(Situation.Rotation);}      
        public void ButtonServe()           { HandlerClickedSituation(Situation.ServeStack); }
        public void ButtonBaseDefense()     { HandlerClickedSituation(Situation.BaseDefense); }
        public void ButtonServeReceive()    { HandlerClickedSituation(Situation.ServeReceiveStack); }
        public void ButtonAttack()          { HandlerClickedSituation(Situation.ReadyToAttack); }

        Situation lastClickedSituation = Situation.None;
        int lastClickedRotation = 0;

        private void UpdateStatusDisplay()
        {
            string message = $"[{currentRotation}] {currentSituation}";

            if((currentSituation != nextSituation) || (currentRotation != nextRotation))
            {
                message += $"\nTransition to : [{nextRotation}] {nextSituation}";
            }

            uiStatusDisplay.text = message;
        }

        private void HandlerClickedSituation(Situation sit)
        {
            HandleClick(false, 0, true, sit);
        }

        private void HandlerClickedRotation(int rot)
        {
            HandleClick(true, rot, false, Situation.None);
        }


        private void HandleClick(bool wasClickedRotation, int rot, bool wasClickedSituation, Situation sit)
        {
            //bool newValueRotation = (lastClickedRotation != rot);
            //bool newValueSituation = (lastClickedSituation != sit);

            // When the situation was clicked and is a new value, then we need to update the next situation to match the clicked situation.
            // When the situation was clicked and is the same value, then we need to update the current situation to match the clicked situation, and then update the next situation to match the current situation.
            // When the rotation was clicked and is a new value, then we need to update the next rotation to match the clicked rotation.
            // When the rotation was clicked and is the same value, then we need to update the current rotation to match the clicked rotation, and then update the next rotation to match the current rotation.

            if (wasClickedSituation)
            { 
                // if the situation clicked doesn't match the next situation, then update the next situation to match the clicked situation.
                if( nextSituation != sit)
                {
                    // This is a new value clicked, so update the next situation to match this value.
                    nextSituation = sit;
                }
                else
                {
                    // clicked matches next, so update the current to match this value
                    currentSituation = sit;
                    currentRotation = nextRotation;

                    nextSituation = currentSituation;
                }
            }

            if (wasClickedRotation)
            {
                if (nextRotation != rot)
                {
                    // This is a new value clicked, so update the next rotation to match this value.
                    nextRotation = rot;
                }
                else
                {
                    // Same was clicked, so update the current to match this value, and then update the next to match the current.
                    currentRotation = rot;
                    currentSituation = nextSituation;
                    //currentSituation = Situation.ServeStack; // When we rotate, we're serving, except for the initial serve receive due to coin toss.

                    nextRotation = currentRotation;

                }
            }

            buttonGroupSituation.SetColorSelection((int)currentSituation, Color.green, (int)nextSituation, Color.red, Color.gray);
            buttonGroupRotation.SetColorSelection(currentRotation - 1, Color.green, nextRotation - 1, Color.red, Color.gray);
            UpdateStatusDisplay();

            // TODO - we may not need this logic since we're using the current and next values to determine the rotation and situation.
            // Update the last clicked values
            if(wasClickedRotation)
            {
                lastClickedRotation = rot;
            }

            if(wasClickedSituation)
            {
                lastClickedSituation = sit;
            }
        }
        #endregion
    }


}
