using System;   // serializable
using System.Collections.Generic;

public enum BeatType {
    Up, Down, Left, Right
}

[Serializable]
public class BeatSequence {
    public BeatType noteType;
    public float interval;
    public float startTime;
    public float endTime;
    public BeatSequence(BeatType t, float i, float s, float e) {
        noteType = t;
        interval = i;
        startTime = s;
        endTime = e;
    }
}

[Serializable]
public class BeatSequenceList {
    public List<BeatSequence> sequences;
    public BeatSequenceList(List<BeatSequence> s) {
        sequences = s;
    }
}

public class BeatInfo {
    public float beat;
    public BeatType beatType;
    public BeatInfo(float b, BeatType t) {
        beat = b;
        beatType = t;
    }
}