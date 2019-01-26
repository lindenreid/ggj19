using System.Collections.Generic;
using UnityEngine;

public class NoteController : MonoBehaviour
{
    public TimingCounter TimingCounter;
    public Transform noteStartLocation;
    public Transform noteEndLocation;
    public GameObject notePrefab;
    public float beatsTillDest = 8.0f; // how long the note bar is
    public float distancePastGoal = 0.2f;

    private List<float> noteSpawnTimes;
    private int nextBeatIndex;
    private float distanceToGoal;
    private float timeFromSpawnToGoal;

    public void Init (List<float> beats) {
        nextBeatIndex = 0;
        distanceToGoal = Mathf.Abs(noteStartLocation.position.x - noteEndLocation.position.x);
        timeFromSpawnToGoal = beatsTillDest / TimingCounter.beatsPerSec;
        
        // loop through every beat in song
        // and calculate spawn time
        noteSpawnTimes = new List<float>();
        foreach(float destinationTime in beats) {
            // don't add any notes that start before the time it takes to get to the goal
            float spawnTime = destinationTime - timeFromSpawnToGoal;
            if(spawnTime >= timeFromSpawnToGoal) {
                noteSpawnTimes.Add(spawnTime);
            }
        }

        /*foreach(float s in noteSpawnTimes) {
            Debug.Log("spawn: " + s);
        }*/
    }

    void Update () {
        if(TimingCounter.AudioSource.time >= noteSpawnTimes[nextBeatIndex]) {
            GameObject noteObj = Instantiate(notePrefab, noteStartLocation) as GameObject;
            Note note = noteObj.GetComponent<Note>();
            note.Init(speed: distanceToGoal / timeFromSpawnToGoal, dest:noteEndLocation.position.x, past:distancePastGoal, timingCounter:TimingCounter);
            nextBeatIndex++;
        }
    }

}
