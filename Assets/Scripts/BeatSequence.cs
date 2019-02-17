using System;   // serializable
using System.Collections.Generic;

public enum BeatType {
    Up = 0,
    Down = 1,
    Left = 2,
    Right = 3
}

[Serializable]
public class BeatSequence {
    public BeatType noteType;
    public bool hasPriority;
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
    public bool hasPriority;
    public float spawnTime;
    public BeatInfo(float b, BeatType t, bool p, float st) {
        beat = b;
        beatType = t;
        hasPriority = p;
        spawnTime = st;
    }
}