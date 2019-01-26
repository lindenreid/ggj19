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

    public float songStartTime = 0.0f;

    public float accuracyGreat;
    public float accuracyOk;

    public int interval = 1; // placeholder interval for one set of beats to hit every Nth beat (interval = N)
    public float beatsPerSec = 1.0f; // number of beats per sec in this song

    private List<float> beats;

    void Start () {
        beats = new List<float>();

        // initialize all of the song time location of beats
        float currentTime = songStartTime;
        while(currentTime <= AudioSource.clip.length) {
            currentTime += interval / beatsPerSec;
            beats.Add(currentTime);
        }

        /*foreach(float f in beats) {
            Debug.Log("b: " + f);
        }*/

        NoteController.Init(beats);
    }

    public Accuracy GetAccuracy () {
        float songPos = AudioSource.time;
        float timingDifference = GetClosestBeatTimeDifference(songPos);

        Accuracy acc = Accuracy.Miss;

        if(timingDifference <= accuracyOk && timingDifference > accuracyGreat) {
            acc = Accuracy.Ok;
        } else if (timingDifference <= accuracyGreat) {
            acc = Accuracy.Great;
        }

        Debug.Log("current time: " + songPos + "; timing diff: " + timingDifference);

        return acc;
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
