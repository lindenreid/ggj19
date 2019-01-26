using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum Accuracy {
    Great, Ok, Miss
}

public class TimingCounter : MonoBehaviour
{
    public AudioSource AudioSource;
    public NoteController NoteController;
    public GameObject missObj;
    public GameObject okObj;
    public GameObject perfObj;

    public float songStartTime = 0.0f;

    public float accuracyGreat;
    public float accuracyOk;
    public float attemptWindow; // how far away (in seconds) from the next beat do we check for accuracy?

    public int interval = 1; // placeholder interval for one set of beats to hit every Nth beat (interval = N)
    public float beatsPerSec = 1.0f; // number of beats per sec in this song
    public int firstBeat = 8;

    private Dictionary<float, float> beats; // maps beat time (sec) to note spawn time (sec)
    public Dictionary<float, float> Beats {
        get { return Beats; }
    }

    private int currentBeatIndex = 0;
    public int CurrentNoteIndex {
        get { return currentBeatIndex; }
    }

    void Start () {
        beats = new Dictionary<float, float>();
        float timeFromSpawnToGoal = NoteController.beatsTillDest / beatsPerSec;

        // initialize all of the song time location of beats
        float currentTime = songStartTime + firstBeat*beatsPerSec; // start at time of first beat
        while(currentTime <= AudioSource.clip.length) {
            float spawnTime = currentTime - timeFromSpawnToGoal;
            if(spawnTime >= timeFromSpawnToGoal) { // don't add any notes that start too early
                beats.Add(currentTime, spawnTime);
                currentTime += interval / beatsPerSec;
            }
        }

        /*foreach(float f in beats) {
            Debug.Log("b: " + f);
        }*/

        NoteController.Init(timeFromSpawnToGoal);
    }

    public void NoteHit () {
        float songPos = AudioSource.time;

        // 1. FIND CLOSEST BEAT AND TIME DIFFERENCE BETWEEN BEAT TIME AND ACTUAL TIME
        var beatTimes = beats.Keys;
        float timingDifference = float.MaxValue;
        float attemptedBeat = -1.0f;

        // could probably make this a binary search since the list is sorted to be faster, but who cares 
        // find the beat start time that's closeset to the input time
        foreach(float beat in beatTimes) { 
            float diff = Mathf.Abs(beat - songPos);
            if(diff < timingDifference) {
                timingDifference = diff;
                attemptedBeat = beat;
            }
        }

        // 1.5. CHECK IF NEXT BEAT IS CLOSE ENOUGH TO BOTHER CHECKING FOR ACCURACY
        if(timingDifference >= attemptWindow) {
            return;
        }

        // 2. GET ACCURACY BASED ON TIME DIFFERENCE
        Accuracy acc = Accuracy.Miss;

        if(timingDifference <= accuracyOk && timingDifference > accuracyGreat) {
            acc = Accuracy.Ok;
        } else if (timingDifference <= accuracyGreat) {
            acc = Accuracy.Great;
        }

        //Debug.Log("current time: " + songPos + "; timing diff: " + timingDifference + "; accuracy: " + acc);

        // 3. DISPLAY RESULTS
        ShowAccuracy(acc);
        NoteController.NoteHit(attemptedBeat);
        
        IncrementBeat();
    }

    public void NoteDied () {
        ShowAccuracy(Accuracy.Miss);

        // tell notecontroller so it can remove the note from the list of current notes
        float beat = GetCurrentBeat();
        NoteController.NoteDied(beat);

        IncrementBeat();
    }

    public float GetCurrentBeat () {
        float[] beatTimes = new float[beats.Keys.Count];
        beats.Keys.CopyTo(beatTimes, 0);
        return beatTimes[currentBeatIndex];
    }

    public float GetBeat (int beatIndex) {
        float[] beatTimes = new float[beats.Keys.Count];
        beats.Keys.CopyTo(beatTimes, 0);
        return beatTimes[beatIndex];
    }

    public float GetBeatSpawnTime (int beatIndex) {
        float beat = GetBeat(beatIndex);
        return beats[beat];
    }

    private void IncrementBeat() {
        currentBeatIndex++;
    }

    private void ShowAccuracy(Accuracy acc) {
        switch(acc) {
            case Accuracy.Great:
                missObj.SetActive(false);
                okObj.SetActive(false);
                perfObj.SetActive(true);
            break;
            case Accuracy.Ok:
                missObj.SetActive(false);
                okObj.SetActive(true);
                perfObj.SetActive(false);
            break;
            case Accuracy.Miss:
                missObj.SetActive(true);
                okObj.SetActive(false);
                perfObj.SetActive(false);
            break;
            default:
                Debug.LogError("accuracy case not handled!");
            break;
        }
    }
}
