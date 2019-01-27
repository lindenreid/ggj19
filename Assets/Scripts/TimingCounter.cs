using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

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
    public Text songTimeDebugText;
    
    public string DataFileName = "sequences.json";

    public float songStartTime = 0.0f;

    public float accuracyGreat;
    public float accuracyOk;
    public float attemptWindow; // how far away (in seconds) from the next beat do we check for accuracy?

    public float beatsPerSec = 1.0f; // number of beats per sec in this song

    private Dictionary<BeatInfo, float> beats; // maps beat time (sec) to note spawn time (sec)
    public Dictionary<BeatInfo, float> Beats {
        get { return Beats; }
    }

    private int currentBeatIndex = 0;
    public int CurrentNoteIndex {
        get { return currentBeatIndex; }
    }

    private List<BeatSequence> beatSequences;

    private bool gameRunning;
    public bool GameRunning {
        get { return gameRunning; }
    }

    void Start () {
        gameRunning = true; 

        ImportBeatSequences();

        beats = new Dictionary<BeatInfo, float>();
        float timeFromSpawnToGoal = NoteController.beatsTillDest / beatsPerSec;

        // initialize all of the song time location of beats
        foreach(BeatSequence seq in beatSequences) {
            float currentTime = seq.startTime + songStartTime; // start at time of first beat

            while(currentTime <= seq.endTime && currentTime <= AudioSource.clip.length) {
                float spawnTime = currentTime - timeFromSpawnToGoal;
                if(spawnTime >= timeFromSpawnToGoal) { // don't add any notes that start too early
                    beats.Add(new BeatInfo(currentTime, seq.noteType, seq.hasPriority), spawnTime);
                    //Debug.Log("added: " + currentTime + ", " + seq.noteType);
                    currentTime += seq.interval / beatsPerSec;
                }
            }
        }
        SortBeats();

        /*foreach(BeatInfo i in beats.Keys) {
            Debug.Log("beat time: " + i.beat + "; type: " + i.beatType);
        }*/

        currentBeatIndex = 0;

        NoteController.Init(timeFromSpawnToGoal);
    }

    void Update () {
        songTimeDebugText.text = AudioSource.time + "";
    }

    public void NoteHit (BeatType beatType) {
        if(!gameRunning) return;

        float songPos = AudioSource.time;
        BeatInfo beatInfo = GetCurrentBeat();
        Accuracy acc = Accuracy.Miss; // guilty until proven innocent

        // 1. FIND TIME DIFFERENCE BETWEEN BEAT TIME AND ACTUAL TIME
        float timingDifference = Mathf.Abs(beatInfo.beat - songPos);

        // 2. CHECK IF NEXT BEAT IS CLOSE ENOUGH TO BOTHER CHECKING FOR ACCURACY
        if(timingDifference >= attemptWindow) {
            return;
        }

        // 3. FIGURE OUT IF CORRECT BUTTON WAS HIT
        if(beatType == beatInfo.beatType) {
            // 4. GET ACCURACY BASED ON TIME DIFFERENCE
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
        if(!gameRunning) return;

        //BeatInfo i = GetBeat(currentBeatIndex);
        //Debug.Log("beat type: " + i.beatType + "; beat: " + i.beat);
        currentBeatIndex++;

        if(currentBeatIndex >= beats.Keys.Count) {
            EndGame();
        }
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
            bool addNewKvp = true;
            // if 2 beats are at the same position,
            // only add new one if if has priority
            if(beatTimes.Contains(kvp.Key.beat)) {
                if(kvp.Key.hasPriority) {
                    // remove old one
                    BeatInfo keyToRemove = newBeats.Keys.FirstOrDefault(b => b.beat == kvp.Key.beat);
                    newBeats.Remove(keyToRemove);
                } else {
                    // don't add new one
                    addNewKvp = false;
                }
            }

            if(addNewKvp) {
                newBeats.Add(kvp.Key, kvp.Value);
                beatTimes.Add(kvp.Key.beat);
            }
        }
        beats = newBeats;
    }

    private void EndGame() {
        gameRunning = false;
        Score.EndGame();
    }
}
