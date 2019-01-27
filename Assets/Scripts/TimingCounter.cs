using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Linq;

public enum Accuracy {
    Great, Ok, Miss
}

public class TimingCounter : MonoBehaviour
{
    public UI UI;
    public AudioSource AudioSource;
    public NoteController NoteController;
    public Score Score;
    public DumplingAnimator DumplingAnimator;
    
    public string DataFileName = "sequences.json";

    public float songStartTime = 0.0f;

    public float accuracyGreat;
    public float accuracyOk;
    public float attemptWindow; // how far away (in seconds) from the next beat do we check for accuracy?

    public float interval = 1; // placeholder interval for one set of beats to hit every Nth beat (interval = N)
    public float beatsPerSec = 1.0f; // number of beats per sec in this song
    public int firstBeat = 8;

    private Dictionary<BeatInfo, float> beats; // maps beat time (sec) to note spawn time (sec)
    public Dictionary<BeatInfo, float> Beats {
        get { return Beats; }
    }

    private int currentBeatIndex = 0;
    public int CurrentNoteIndex {
        get { return currentBeatIndex; }
    }

    private List<BeatSequence> beatSequences;

    void Start () {
        ImportBeatSequences();

        beats = new Dictionary<BeatInfo, float>();
        float timeFromSpawnToGoal = NoteController.beatsTillDest / beatsPerSec;

        // initialize all of the song time location of beats
        foreach(BeatSequence seq in beatSequences) {
            float currentTime = seq.startTime + songStartTime + firstBeat*beatsPerSec; // start at time of first beat

            while(currentTime <= seq.endTime && currentTime <= AudioSource.clip.length) {
                float spawnTime = currentTime - timeFromSpawnToGoal;
                if(spawnTime >= timeFromSpawnToGoal) { // don't add any notes that start too early
                    beats.Add(new BeatInfo(currentTime, seq.noteType), spawnTime);
                    //Debug.Log("added: " + currentTime + ", " + seq.noteType);
                    currentTime += seq.interval / beatsPerSec;
                }
            }
        }
        SortBeats();

        currentBeatIndex = 0;

        NoteController.Init(timeFromSpawnToGoal);
    }

    public void NoteHit (BeatType beatType) {
        float songPos = AudioSource.time;
        BeatInfo beatInfo = GetCurrentBeat();
        Accuracy acc = Accuracy.Miss; // guilty until proven innocent

        // 0. FIGURE OUT IF CORRECT BUTTON WAS HIT
        if(beatType == beatInfo.beatType) {
            // 1. FIND TIME DIFFERENCE BETWEEN BEAT TIME AND ACTUAL TIME
            float timingDifference = Mathf.Abs(beatInfo.beat - songPos);

            // 2. CHECK IF NEXT BEAT IS CLOSE ENOUGH TO BOTHER CHECKING FOR ACCURACY
            if(timingDifference >= attemptWindow) {
                return;
            }

            // 3. GET ACCURACY BASED ON TIME DIFFERENCE
            if(timingDifference <= accuracyOk && timingDifference > accuracyGreat) {
                acc = Accuracy.Ok;
            } else if (timingDifference <= accuracyGreat) {
                acc = Accuracy.Great;
            }

            //Debug.Log("current time: " + songPos + "; timing diff: " + timingDifference + "; accuracy: " + acc);
        }

        // 4. DISPLAY RESULTS
        UI.ShowAccuracy(acc);
        NoteController.NoteHit(beatInfo.beat);
        Score.NoteHit(acc);

        // animate dumpling if we hit the note!
        if(acc != Accuracy.Miss) {
            DumplingAnimator.IncrementFrame();
        }
        
        IncrementBeat();
    }

    public void NoteDied () {
        UI.ShowAccuracy(Accuracy.Miss);

        // tell score
        Score.NoteHit(Accuracy.Miss);

        // tell notecontroller so it can remove the note from the list of current notes
        float beat = GetCurrentBeat().beat;
        NoteController.NoteDied(beat);

        IncrementBeat();
    }

// I KNOW THERES A LOT OF COPY PASTE LEAVE ME ALONE
    public BeatInfo GetCurrentBeat () {
        return GetBeat(currentBeatIndex);
    }

    public BeatInfo GetBeat (int beatIndex) {
        BeatInfo[] beatTimes = new BeatInfo[beats.Keys.Count];
        beats.Keys.CopyTo(beatTimes, 0);
        return beatTimes[beatIndex];
    }

    public float GetBeatSpawnTime (int beatIndex) {
        BeatInfo[] beatTimes = new BeatInfo[beats.Keys.Count];
        beats.Keys.CopyTo(beatTimes, 0);
        BeatInfo beatInfo = beatTimes[beatIndex];
        return beats[beatInfo];
    }

    private void IncrementBeat() {
        //BeatInfo i = GetBeat(currentBeatIndex);
        //Debug.Log("beat type: " + i.beatType + "; beat: " + i.beat);
        currentBeatIndex++;
    }

    // THIS SHOUDL BE SOMEWHERE ELSE BUT IDC
    private void ImportBeatSequences () {
        string filePath = Path.Combine (Application.streamingAssetsPath, DataFileName);

		if (File.Exists (filePath)) {
			string dataString = File.ReadAllText (filePath);
			BeatSequenceList listobj = JsonUtility.FromJson<BeatSequenceList>(dataString);
            beatSequences = listobj.sequences;
		} else {
			Debug.LogError("Unable to find beat sequence data file.");
        }

        /*foreach(BeatSequence s in beatSequences) {
            Debug.Log("sequence type: " + s.noteType);
        }*/
    }

    // sort beats by spawn time
    private void SortBeats () {
        Dictionary<BeatInfo, float> newBeats = new Dictionary<BeatInfo, float>();
        List<float> beatTimes = new List<float>();
        foreach(KeyValuePair<BeatInfo, float> kvp in beats.OrderBy(key => key.Value)) {
            if(!beatTimes.Contains(kvp.Key.beat)) {
                newBeats.Add(kvp.Key, kvp.Value);
                beatTimes.Add(kvp.Key.beat);
            }
        }
        beats = newBeats;
    }
}
