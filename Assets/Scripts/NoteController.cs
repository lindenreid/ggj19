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
    private int nextBeatToSpawnIndex = 0; // tracking which beat to SPAWN next, *NOT* which beat to track for hit accuracy
    
    private Dictionary<float, Note> notes; // maps beat time to Note object

    public void Init (float timeFromSpawnToGoal) {
        this.timeFromSpawnToGoal = timeFromSpawnToGoal;
        distanceToGoal = Mathf.Abs(noteStartLocation.position.x - noteEndLocation.position.x);

        notes = new Dictionary<float, Note>();
    }

    void Update () {
        if(TimingCounter.AudioSource.time >= TimingCounter.GetBeatSpawnTime(nextBeatToSpawnIndex)) {
            SpawnNewNote(TimingCounter.GetBeat(nextBeatToSpawnIndex));
            nextBeatToSpawnIndex++;
        }
    }

    public void NoteHit(float beat) {
        if(notes.ContainsKey(beat)) {
            Note note = notes[beat];
            //Debug.Log("note attempted: " + note.gameObject.name);
            note.PlayHitAnimation();
        }
        else {
            Debug.LogError("beat " + beat + " not found!");
        }
    }

    public void NoteDied(float beat) {
        notes.Remove(beat);
    }

    public void SpawnNewNote(float beat) {
        GameObject noteObj = Instantiate(notePrefab, noteStartLocation) as GameObject;
        Note note = noteObj.GetComponent<Note>();
        note.Init(beat:beat, speed:distanceToGoal / timeFromSpawnToGoal, dest:noteEndLocation.position.x, past:distancePastGoal, timingCounter:TimingCounter);

        // for debug purposes
        noteObj.name = "Beat " + beat;

        notes.Add(beat, note);
    }

}
