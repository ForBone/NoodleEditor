using UnityEngine;
using System.Collections.Generic;
using TMPro;

public class BeatGen : MonoBehaviour
{
    public GameObject prefab;  // The prefab to instantiate
    public GameObject beatNumberPrefab;  // The prefab for the beat number
    public float distanceBetweenPrefabs = 2.0f;  // Distance between each prefab
    public List<Vector3> beatPositions = new List<Vector3>();  // List to store beat positions
    public float SnapInterval = 1f;
    public float disableThreshold = -20.0f; // Define your own threshold value

    private Vector3 startPosition;  // Store the initial start position for resetting
    private List<GameObject> instantiatedBeats = new List<GameObject>();  // List to store instantiated beat objects
    private List<GameObject> instantiatedBeatNumbers = new List<GameObject>();  // List to store instantiated beat number objects

    void Start()
    {
        startPosition = transform.position;
    }

    // Method to generate line up to a specific number of beats
    public void GenerateLine(int numberOfBeats)
    {
        ClearBeats();

        beatPositions.Clear();
        for (int i = 0; i <= numberOfBeats; i++)
        {
            // Calculate the position for the new prefab
            Vector3 position = startPosition + new Vector3(0, 0, i * distanceBetweenPrefabs);

            // Instantiate the prefab at the calculated position
            GameObject beat = Instantiate(prefab, position, Quaternion.identity);
            instantiatedBeats.Add(beat);

            // Add the position to the list of beat positions
            beatPositions.Add(position);

            // Instantiate the beat number prefab
            Quaternion beatq = Quaternion.Euler(90, 0, 0);
            Vector3 numberPosition = position + new Vector3((float)2.5, 0, 0);  // Offset to place the number to the side
            GameObject beatNumber = Instantiate(beatNumberPrefab, numberPosition, beatq);
            instantiatedBeatNumbers.Add(beatNumber);

            // Set the text of the beat number
            beatNumber.GetComponent<TextMeshPro>().text = i.ToString();
        }
    }

    // Method to clear previously instantiated beats and beat numbers
    private void ClearBeats()
    {
        foreach (GameObject beat in instantiatedBeats)
        {
            Destroy(beat);
        }
        instantiatedBeats.Clear();

        foreach (GameObject beatNumber in instantiatedBeatNumbers)
        {
            Destroy(beatNumber);
        }
        instantiatedBeatNumbers.Clear();
    }

    // Method to handle scroll input
    public void ScrollBeats(float scrollAmount)
    {
        // Calculate the new start position based on the scroll amount
        float scrollOffset = scrollAmount * distanceBetweenPrefabs * SnapInterval;
        Vector3 newStartPosition = startPosition + new Vector3(0, 0, scrollOffset);

        // Prevent scrolling forwards if beat line 0 is at x=0, y=0, z=0
        if (newStartPosition.z <= 0)
        {
            startPosition = newStartPosition;
            UpdateBeatPositions();
            UpdateBeatVisibility();
        }
    }

    // Method to update beat positions based on the new start position
    private void UpdateBeatPositions()
    {
        for (int i = 0; i < beatPositions.Count; i++)
        {
            Vector3 newPosition = startPosition + new Vector3(0, 0, i * distanceBetweenPrefabs);
            beatPositions[i] = newPosition;
            instantiatedBeats[i].transform.position = newPosition;

            // Update the position of the beat number and reset its rotation
            Quaternion beatq = Quaternion.Euler(90, 0, 0);
            Vector3 numberPosition = newPosition + new Vector3((float)2.5, 0, 0);  // Offset to place the number to the side
            instantiatedBeatNumbers[i].transform.position = numberPosition;
            instantiatedBeatNumbers[i].transform.rotation = beatq;  // Reset rotation to default
        }
    }

    private void UpdateBeatVisibility()
    {
        foreach (GameObject beat in instantiatedBeats)
        {
            if (beat.transform.position.z < disableThreshold)
            {
                beat.SetActive(true);
            }
            else
            {
                beat.SetActive(true);
            }
        }

        foreach (GameObject beatNumber in instantiatedBeatNumbers)
        {
            if (beatNumber.transform.position.z < disableThreshold)
            {
                beatNumber.SetActive(false);
            }
            else
            {
                beatNumber.SetActive(true);
            }
        }
    }

    public void SetSnapInterval(float interval)
    {
        SnapInterval = Mathf.Max(0.1f, interval);  // Ensure snap interval is not zero or negative
    }
}
