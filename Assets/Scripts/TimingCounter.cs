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

    public int interval = 1; // placeholder interval for one set of beats to hit every Nth beat (interval = N)
    public float beatsPerSec = 1.0f; // number of beats per sec in this song
    public int firstBeat = 8;

    private List<float> beats;
    private bool attemptedCurrentNote = false;

    void Start () {
        beats = new List<float>();

        // initialize all of the song time location of beats
        float currentTime = songStartTime + firstBeat*beatsPerSec; // start at time of first beat
        while(currentTime <= AudioSource.clip.length) {
            beats.Add(currentTime);
            currentTime += interval / beatsPerSec;
        }

        /*foreach(float f in beats) {
            Debug.Log("b: " + f);
        }*/

        NoteController.Init(beats);
    }

    public void NoteHit () {
        Accuracy acc = GetAccuracy();
        ShowAccuracy(acc);
        attemptedCurrentNote = true;
    }

    public void NoteDied () {
        if(!attemptedCurrentNote) {
            ShowAccuracy(Accuracy.Miss);
        }
        attemptedCurrentNote = false;
    }

    private Accuracy GetAccuracy () {
        float songPos = AudioSource.time;
        float timingDifference = GetClosestBeatTimeDifference(songPos);

        Accuracy acc = Accuracy.Miss;

        if(timingDifference <= accuracyOk && timingDifference > accuracyGreat) {
            acc = Accuracy.Ok;
        } else if (timingDifference <= accuracyGreat) {
            acc = Accuracy.Great;
        }

        //Debug.Log("current time: " + songPos + "; timing diff: " + timingDifference + "; accuracy: " + acc);

        return acc;
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

    private float GetClosestBeatTimeDifference (float time) {
        float smallestDiff = Mathf.Abs(beats[0] - time);

        // could probably make this a binary search since the list is sorted to be faster, but who cares 
        for(int i = 1; i < beats.Count; i++) { 
            float diff = Mathf.Abs(beats[i] - time);
            if(diff < smallestDiff)
                smallestDiff = diff;
        }

        return smallestDiff;
    }
}
