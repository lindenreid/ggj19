using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public enum Accuracy {
    Great, Ok, Miss
}

public class TimingCounter : MonoBehaviour
{
    public UI UI;
    public AudioSource AudioSource;
    public AudioSource NoteHitAudioSource;
    public AudioClip NoteHitClip;
    public NoteController NoteController;
    public Score Score;
    public DumplingAnimator DumplingAnimator;
    public Text songTimeDebugText;

    public float songStartTime = 0.0f;

    private string dataFileName = "sequences";

    public float accuracyGreat;
    public float accuracyOk;
    public float attemptWindow; // how far away (in seconds) from the next beat do we check for accuracy?

    public float beatsPerSec = 1.0f; // number of beats per sec in this song

    private List<BeatInfo> beats; // maps beat time (sec) to note spawn time (sec)
    public List<BeatInfo> Beats {
        get { return beats; }
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
        LoadBeatSequences();

        beats = new List<BeatInfo>();
        float timeFromSpawnToGoal = NoteController.beatsTillDest / beatsPerSec;

        // initialize all of the song time location of beats
        foreach(BeatSequence seq in beatSequences) {
            float currentTime = seq.startTime + songStartTime; // start at time of first beat

            while(currentTime <= seq.endTime && currentTime <= AudioSource.clip.length) {
                float spawnTime = currentTime - timeFromSpawnToGoal;
                if(spawnTime >= timeFromSpawnToGoal) { // don't add any notes that start too early
                    beats.Add(new BeatInfo(currentTime, seq.noteType, seq.hasPriority, spawnTime));
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
        gameRunning = true; 
        NoteController.Init(timeFromSpawnToGoal);
    }

    private void LoadBeatSequences () {
        beatSequences = new List<BeatSequence>();

        var textFile = Resources.Load<TextAsset>(dataFileName);
        if(textFile == null) {
            Debug.LogError("unable to find file: " + dataFileName);
            return;
        }

        string text = textFile.text;
        if(!string.IsNullOrEmpty(text)) {
            BeatSequenceList listobj = JsonUtility.FromJson<BeatSequenceList>(text);
            beatSequences = listobj.sequences;

            /*foreach(BeatSequence s in beatSequences) {
                Debug.Log("sequence type: " + s.noteType);
            }*/
        } else {
            Debug.LogError("Beat sequence data file empty.");
        }
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
            NoteHitAudioSource.PlayOneShot(NoteHitClip, 1);
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
        if(beatIndex >= beats.Count || beatIndex < 0) return null;
        return beats[beatIndex];
    }

    public float GetBeatSpawnTime (int beatIndex) {
        if(beatIndex >= beats.Count || beatIndex < 0) return -1.0f;
        return beats[beatIndex].spawnTime;
    }

    private void IncrementBeat() {
        if(!gameRunning) return;

        //BeatInfo i = GetBeat(currentBeatIndex);
        //Debug.Log("beat type: " + i.beatType + "; beat: " + i.beat);
        currentBeatIndex++;

        if(currentBeatIndex >= beats.Count) {
            EndGame();
        }
    }

    // sort beats by spawn time
    public void SortBeats () {
        List<BeatInfo> newBeats = new List<BeatInfo>();
        foreach(BeatInfo beatInfo in beats.OrderBy(b => b.spawnTime)) {
            bool addNewBeat = true;
            // if 2 beats are at the same position,
            // only add new one if if has priority
            if(newBeats.Any(b => b.beat == beatInfo.beat)) {
                if(beatInfo.hasPriority) {
                    // remove old one
                    BeatInfo beatToRemove = newBeats.FirstOrDefault(b => b.beat == beatInfo.beat);
                    newBeats.Remove(beatToRemove);
                } else {
                    // don't add new one
                    addNewBeat = false;
                }
            }

            if(addNewBeat) {
                newBeats.Add(beatInfo);
            }
        }
        beats = newBeats;
    }

    private void EndGame() {
        gameRunning = false;
        Score.EndGame();
    }
}
