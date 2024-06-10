using System;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

// Class to represent a single note
[System.Serializable]
public class Note
{
    public float b;
    public int x;
    public int y;
    public int c;
    public int d;
}
[System.Serializable]
public class Chain
{
    public float b; // beat
    public int x;
    public int y;
    public int c;
    public int tx;
    public int ty;
    public float tb;
    public int d;
    public int sc;
    public float s;
}

// Class to represent the list of notes
[System.Serializable]
public class NoteData
{
    public List<Note> colorNotes;
}
[System.Serializable]
public class ChainData
{
    public List<Chain> burstSliders;
}

public class NoteSpawner : MonoBehaviour
{
    // Assign this in the Unity Editor
    public GameObject notePrefab;
    public GameObject notePrefabDot;
    public GameObject noteHalf;
    public GameObject noteSlider;
    public GameObject noteHalfDot;
    public Material red;
    public Material blue;

    // Reference to the LineGenerator to get beat positions
    public BeatGen lineGenerator;

    // List to store the instantiated notes
    private List<GameObject> spawnedNotes = new List<GameObject>();
    private List<GameObject> spawnedChains = new List<GameObject>();

    // JSON data as a string
    private string jsonData = @"
    {
                      ""burstSliders"":[{""b"":2,""c"":1,""x"":1,""y"":1,""d"":6,""tb"":2.5,""tx"":0,""ty"":0,""sc"":4,""s"":1}]
    }";

    void Start()
    {
        // Parse the JSON data
        NoteData noteData = JsonUtility.FromJson<NoteData>(jsonData);
        ChainData chainData = JsonUtility.FromJson<ChainData>(jsonData);

        Console.WriteLine(chainData);

        // Determine the highest beat value
        int highestBeat = 0;
        foreach (Note note in noteData.colorNotes)
        {
            if (note.b > highestBeat)
            {
                highestBeat = Mathf.CeilToInt(note.b);
            }
        }

        foreach (Chain chain in chainData.burstSliders)
        {
            if (chain.b > highestBeat)
            {
                highestBeat = Mathf.CeilToInt(chain.b);
            }
        }

        // Generate line positions up to the highest beat value
        lineGenerator.GenerateLine(highestBeat);

        // Loop through each note and create a corresponding GameObject
        foreach (Note note in noteData.colorNotes)
        {
            CreateNoteObject(note);
        }

        foreach (Chain chain in chainData.burstSliders)
        {
            CreateChainObject(chain);
            
        }
    }

    void CreateNoteObject(Note note)
    {
        GameObject noteObject = InstantiateNoteAtPosition(note);
        noteObject.AddComponent<NoteDataHolder>().note = note;
        spawnedNotes.Add(noteObject);
        
    }

    void CreateChainObject(Chain chain)
    {
        GameObject chainObject = InstantiateChainAtPosition(chain);
        chainObject.AddComponent<ChainDataHolder>().chain = chain;
        spawnedChains.Add(chainObject);
    }

    GameObject InstantiateNoteAtPosition(Note note)
    {
        int x = 0, y = 0, z = 0;
        if (note.d == 0) { z = 180; }
        if (note.d == 1) { z = 0; }
        if (note.d == 2) { z = 270; }
        if (note.d == 3) { z = 90; }
        if (note.d == 4) { z = 225; }
        if (note.d == 5) { z = 135; }
        if (note.d == 6) { z = -45; }
        if (note.d == 7) { z = 45; }

        Quaternion q = Quaternion.Euler(x, y, z);

        // Find the corresponding beat positions for interpolation
        Vector3 beatPositionStart = Vector3.zero;
        Vector3 beatPositionEnd = Vector3.zero;
        if (lineGenerator != null)
        {
            int startIndex = Mathf.FloorToInt(note.b);
            int endIndex = Mathf.CeilToInt(note.b);

            if (startIndex < lineGenerator.beatPositions.Count)
            {
                beatPositionStart = lineGenerator.beatPositions[startIndex];
            }
            if (endIndex < lineGenerator.beatPositions.Count)
            {
                beatPositionEnd = lineGenerator.beatPositions[endIndex];
            }
        }

        // Interpolate between the start and end positions
        float t = note.b - Mathf.Floor(note.b);
        Vector3 beatPosition = Vector3.Lerp(beatPositionStart, beatPositionEnd, t);

        // Offset the note position based on x and y
        Vector3 notePosition = beatPosition + new Vector3((float)(note.x - 1.5), (float)(note.y + 0.4), 0);

        GameObject noteObj = notePrefab;
        if (note.d == 8) { noteObj = notePrefabDot; }
        if (note.c == 0) { noteObj.GetComponentInChildren<MeshRenderer>().material = red; }
        if (note.c == 1) { noteObj.GetComponentInChildren<MeshRenderer>().material = blue; }

        return Instantiate(noteObj, notePosition, q);
    }
    GameObject InstantiateChainAtPosition(Chain chain)
    {
        int x = -90, y = -90, z = 90;
        if (chain.d == 0) { x += 180; }
        if (chain.d == 1) { x += 0; }
        if (chain.d == 2) { x += 270; }
        if (chain.d == 3) { x += 90; }
        if (chain.d == 4) { x += 225; }
        if (chain.d == 5) { x += 135; }
        if (chain.d == 6) { x += -45; }
        if (chain.d == 7) { x += 45; }

        Quaternion q = Quaternion.Euler(x, y, z);

        // Find the corresponding beat positions for interpolation
        Vector3 beatPositionStart = Vector3.zero;
        Vector3 beatPositionEnd = Vector3.zero;
        if (lineGenerator != null)
        {
            int startIndex = Mathf.FloorToInt(chain.b);
            int endIndex = Mathf.CeilToInt(chain.b);

            if (startIndex < lineGenerator.beatPositions.Count)
            {
                beatPositionStart = lineGenerator.beatPositions[startIndex];
            }
            if (endIndex < lineGenerator.beatPositions.Count)
            {
                beatPositionEnd = lineGenerator.beatPositions[endIndex];
            }
        }

        // Interpolate between the start and end positions
        float t = chain.b - Mathf.Floor(chain.b);
        Vector3 beatPosition = Vector3.Lerp(beatPositionStart, beatPositionEnd, t);

        // Offset the note position based on x and y
        Vector3 chainPosition = beatPosition + new Vector3((float)(chain.x - 1.5), (float)(chain.y + 0.4), 0);

        GameObject chainObj = noteHalf;
        if (chain.d == 8) { chainObj = noteHalfDot; }
        if (chain.c == 0) { chainObj.GetComponentInChildren<MeshRenderer>().material = red; }
        if (chain.c == 1) { chainObj.GetComponentInChildren<MeshRenderer>().material = blue; }

        return Instantiate(chainObj, chainPosition, q);
    }

    void Update()
    {
        // Check for scroll input
        float scroll = -Input.GetAxis("Mouse ScrollWheel");
        if (scroll != 0)
        {
            if (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl))
            {
                // Change snap interval with Ctrl + Scroll
                lineGenerator.SetSnapInterval(lineGenerator.SnapInterval + (scroll > 0 ? 0.1f : -0.1f));
            }
            else
            {
                lineGenerator.ScrollBeats(scroll);
                UpdateNotePositions();
                UpdateChainPositions();
                UpdateNoteVisibility();
                UpdateChainVisibility();
            }
        }
    }

    void UpdateNotePositions()
    {
        foreach (GameObject noteObject in spawnedNotes)
        {
            Note note = noteObject.GetComponent<NoteDataHolder>().note;
            UpdateNotePosition(noteObject, note);
        }
    }
    void UpdateChainPositions()
    {
        foreach (GameObject chainObject in spawnedChains)
        {
            Chain chain = chainObject.GetComponent<ChainDataHolder>().chain;
            UpdateChainPosition(chainObject, chain);
        }
    }
    void UpdateChainPosition(GameObject chainObject, Chain chain)
    {
        // Find the corresponding beat positions for interpolation
        Vector3 beatPositionStart = Vector3.zero;
        Vector3 beatPositionEnd = Vector3.zero;
        if (lineGenerator != null)
        {
            int startIndex = Mathf.FloorToInt(chain.b);
            int endIndex = Mathf.CeilToInt(chain.b);

            if (startIndex < lineGenerator.beatPositions.Count)
            {
                beatPositionStart = lineGenerator.beatPositions[startIndex];
            }
            if (endIndex < lineGenerator.beatPositions.Count)
            {
                beatPositionEnd = lineGenerator.beatPositions[endIndex];
            }
        }

        // Interpolate between the start and end positions
        float t = chain.b - Mathf.Floor(chain.b);
        Vector3 beatPosition = Vector3.Lerp(beatPositionStart, beatPositionEnd, t);

        // Offset the note position based on x and y
        Vector3 chainPosition = beatPosition + new Vector3((float)(chain.x - 1.5), (float)(chain.y + 0.4), 0);

        chainObject.transform.position = chainPosition;
    }

    void UpdateNotePosition(GameObject noteObject, Note note)
    {
        // Find the corresponding beat positions for interpolation
        Vector3 beatPositionStart = Vector3.zero;
        Vector3 beatPositionEnd = Vector3.zero;
        if (lineGenerator != null)
        {
            int startIndex = Mathf.FloorToInt(note.b);
            int endIndex = Mathf.CeilToInt(note.b);

            if (startIndex < lineGenerator.beatPositions.Count)
            {
                beatPositionStart = lineGenerator.beatPositions[startIndex];
            }
            if (endIndex < lineGenerator.beatPositions.Count)
            {
                beatPositionEnd = lineGenerator.beatPositions[endIndex];
            }
        }

        // Interpolate between the start and end positions
        float t = note.b - Mathf.Floor(note.b);
        Vector3 beatPosition = Vector3.Lerp(beatPositionStart, beatPositionEnd, t);

        // Offset the note position based on x and y
        Vector3 notePosition = beatPosition + new Vector3((float)(note.x - 1.5), (float)(note.y + 0.4), 0);

        noteObject.transform.position = notePosition;
    }

    void UpdateNoteVisibility()
    {
        float disableThreshold = -20.0f; // Define your own threshold value
        float EnableThreshold = 50.0f;

        foreach (GameObject noteObject in spawnedNotes)
        {
            if (noteObject.transform.position.z < disableThreshold || noteObject.transform.position.z > EnableThreshold)
            {
                noteObject.SetActive(false);
            }
            else
            {
                noteObject.SetActive(true);
            }
        }
    }
    void UpdateChainVisibility()
    {
        float disableThreshold = -20.0f; // Define your own threshold value
        float EnableThreshold = 50.0f;

        foreach (GameObject chainObject in spawnedChains)
        {
            if (chainObject.transform.position.z < disableThreshold || chainObject.transform.position.z > EnableThreshold)
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
