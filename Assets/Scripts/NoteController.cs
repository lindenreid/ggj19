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

    private float distanceToGoal;
    private float timeFromSpawnToGoal;
    private int nextBeatIndex = 0;

    public void Init (float timeFromSpawnToGoal) {
        this.timeFromSpawnToGoal = timeFromSpawnToGoal;
        distanceToGoal = Mathf.Abs(noteStartLocation.position.x - noteEndLocation.position.x);
    }

    void Update () {
        if(TimingCounter.AudioSource.time >= TimingCounter.GetBeatSpawnTime(nextBeatIndex)) {
            SpawnNewNote();
            nextBeatIndex++;
        }
    }

    public void SpawnNewNote() {
        GameObject noteObj = Instantiate(notePrefab, noteStartLocation) as GameObject;
        Note note = noteObj.GetComponent<Note>();
        note.Init(speed: distanceToGoal / timeFromSpawnToGoal, dest:noteEndLocation.position.x, past:distancePastGoal, timingCounter:TimingCounter);
    }

}
