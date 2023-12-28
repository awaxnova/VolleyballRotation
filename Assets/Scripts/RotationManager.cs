using DG.Tweening;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotationManager : MonoBehaviour
{

    public GameObject[] playerPositions = new GameObject[6];

    private GameObject[] initialPositions = new GameObject[6];

    public GameObject[] decimalPrefabs = new GameObject[10];

    public GameObject currentRotationPosition;

    [ShowInInspector, Range(1,6)]
    [InlineButton("Rotate")]
    [InlineButton("BackRotate")]
    public int currentRotation = 1;
    private int lastRotation = 0;
    private void Rotate() { currentRotation = currentRotation == 6? 1 : Mathf.Clamp(currentRotation + 1, 1, 6); }
    private void BackRotate() { currentRotation = currentRotation == 1? 6 : Mathf.Clamp(currentRotation - 1, 1, 6); }

    // Start is called before the first frame update
    void Start()
    {
        // Copy each of the gameobjects from the playerPositions array to the initialPositions array
        for(int i = 0; i < playerPositions.Length; i++)
        {
            initialPositions[i] = new GameObject($"initialPos[{i}]");
            initialPositions[i].transform.position = playerPositions[i].transform.position;
            initialPositions[i].transform.rotation = playerPositions[i].transform.rotation;
            initialPositions[i].transform.localScale = playerPositions[i].transform.localScale;

        }
    }

    // Update is called once per frame
    void Update()
    {
        // if the current rotation is different from the last rotation
        // then change the position of the players to match the initialPosition of the current rotation

        if(currentRotation != lastRotation)
        {
            for(int i = 0; i < playerPositions.Length; i++)
            {

                // calculate the index of the next position, which should be the currentRotation minus the i value, but modulo 6.
                int newPositionIndex = UpdatedPositionIndex(currentRotation, i);

                
                Debug.Log($"[{i}]: newPositionIndex={newPositionIndex}, currentRotation={currentRotation}");

                // I need to tween the position of the playerPositions[i] to the initialPositions[newPositionIndex]

                // Move this GameObject to the targetTransform's position over 0.4 second
                playerPositions[i].transform.DOMove(initialPositions[newPositionIndex].transform.position, 0.4f).SetEase(Ease.Linear)
                    .OnComplete(() => { 
                        playerPositions[i].transform.rotation = initialPositions[newPositionIndex].transform.rotation;
                        playerPositions[i].transform.localScale = initialPositions[newPositionIndex].transform.localScale;
                    }
                    );

                FaceCamera(playerPositions[i]);
            }

            // Enable the decimalPrefabs[currentRotation] and disable the rest, at the location of the currentRotationPosition
            for(int j = 0; j < decimalPrefabs.Length; j++)
            {
                if (decimalPrefabs[j] != null)
                { 
                    decimalPrefabs[j].SetActive(j == currentRotation);
                    decimalPrefabs[j].transform.position = currentRotationPosition.transform.position;
                    FaceCamera(decimalPrefabs[j]);
                }
            }

        }


        lastRotation = currentRotation;
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

    private void FaceCamera(GameObject objToTurn)
    {
        // Calculate the direction from this GameObject to the camera
        Vector3 lookDirection = Camera.main.transform.position - objToTurn.transform.position;

        // Adjust the rotation to look at the camera while keeping the Z-axis facing it
        objToTurn.transform.rotation = Quaternion.LookRotation(lookDirection, Vector3.up);
    }

}
