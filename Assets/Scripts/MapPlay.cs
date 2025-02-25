using System.Collections;
using UnityEngine;

public class MapPlay : MonoBehaviour
{
    float bpm = 120.0f;
    float beatInterval; // Time interval between beats in seconds
    float scrollSpeed; // Speed to scroll through beats

    public GameObject notesParent; // Assign this in the Inspector
    public GameObject beatsParent; // Assign this in the Inspector

    private bool isMoving = false; // Tracks whether the notes/beats are currently moving
    private float currentScrollAmount = 0f; // Tracks the current scroll amount

    void Start()
    {
        beatInterval = 60.0f / bpm; // Calculate the beat interval based on BPM
        scrollSpeed = 1.0f / beatInterval; // Calculate the scroll speed
    }

    void Update()
    {
        // Toggle movement when spacebar is pressed
        if (Input.GetKeyDown(KeyCode.Space))
        {
            isMoving = !isMoving; // Toggle movement state

            if (!isMoving)
            {
                // Snap to the nearest beat when movement stops
                SnapToNearestBeat();
            }
        }

        // Move the notes and beats if isMoving is true
        if (isMoving)
        {
            float scrollAmount = scrollSpeed * Time.deltaTime; // Amount to scroll this frame
            currentScrollAmount += scrollAmount; // Accumulate the scroll amount

            // Move the parent GameObjects
            notesParent.transform.position -= new Vector3(0, 0, scrollAmount);
            beatsParent.transform.position -= new Vector3(0, 0, scrollAmount);
        }

        // Update visibility of notes and chains
        UpdateNoteVisibility();
        UpdateChainVisibility();
    }

    void UpdateNoteVisibility()
    {
        float disableThreshold = -20.0f; // Define your own threshold value
        float enableThreshold = 50.0f;

        NoteSpawner noteSpawner = FindObjectOfType<NoteSpawner>();
        if (noteSpawner != null)
        {
            foreach (GameObject noteObject in noteSpawner.spawnedNotes)
            {
                if (noteObject.transform.position.z < disableThreshold || noteObject.transform.position.z > enableThreshold)
                {
                    noteObject.SetActive(false);
                }
                else
                {
                    noteObject.SetActive(true);
                }
            }
        }
    }

    void UpdateChainVisibility()
    {
        float disableThreshold = -20.0f; // Define your own threshold value
        float enableThreshold = 50.0f;

        NoteSpawner noteSpawner = FindObjectOfType<NoteSpawner>();
        if (noteSpawner != null)
        {
            foreach (GameObject chainObject in noteSpawner.spawnedChains)
            {
                if (chainObject.transform.position.z < disableThreshold || chainObject.transform.position.z > enableThreshold)
                {
                    chainObject.SetActive(false);
                }
                else
                {
                    chainObject.SetActive(true);
                }
            }
        }
    }

    void SnapToNearestBeat()
    {
        // Calculate the nearest beat based on the current scroll amount
        float beatSnap = Mathf.Round(currentScrollAmount / beatInterval) * beatInterval;

        // Adjust the positions of the parent GameObjects to snap to the nearest beat
        notesParent.transform.position = new Vector3(0, 0, -beatSnap);
        beatsParent.transform.position = new Vector3(0, 0, -beatSnap);

        // Reset the current scroll amount to the snapped position
        currentScrollAmount = beatSnap;
    }
}