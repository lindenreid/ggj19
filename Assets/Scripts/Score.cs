﻿using UnityEngine;

public class Score : MonoBehaviour
{
    public UI UI;

    public float pointsPerOk = 1.0f;
    public float pointsPerPerfect = 1.5f;
    public int comboLostPerMiss = 5;
    public int maxCombo = 50;
    public float comboBonusTime = 2.0f;
    public float comboMultiplier = 2.0f;

    private float score;
    private int combo;
    private float comboTimeLeft;
    private bool comboZone;
    private int dumplingsFinished;
    private int longestStreak;
    private int currentStreak;
    
    private int numPerf;
    private int numOk;
    private int numMiss;

    void Start () {
        Init();
    }

    public void Init () {
        score = 0.0f;
        combo = 0;
        comboTimeLeft = 0.0f;
        comboZone = false;
        dumplingsFinished = 0;
        longestStreak = 0;
        currentStreak = 0;
        numMiss = 0;
        numOk = 0;
        numPerf = 0;
    }

    void Update () {
        if(comboZone) {
            comboTimeLeft -= Time.deltaTime;
            combo = Clamp(combo, 0, maxCombo);
            UI.UpdateCombo((float)comboTimeLeft/(float)comboBonusTime);

            if(comboTimeLeft <= 0.0f) {
                EndComboZone();
            }
        }
    }

    public void NoteHit (Accuracy acc) {
        float pointAdd = 0;
        switch(acc) {
            case Accuracy.Miss:
                if(!comboZone) combo -= comboLostPerMiss;
                if(currentStreak > longestStreak) longestStreak = currentStreak;
                currentStreak = 0;
                numMiss++;
            break;
            case Accuracy.Ok:
                pointAdd = pointsPerOk;
                if(!comboZone) combo ++;
                currentStreak++;
                numOk++;
            break;
            case Accuracy.Great:
                pointAdd = pointsPerPerfect;
                if(!comboZone) combo ++;
                currentStreak++;
                numPerf++;
            break;
            default:
                Debug.LogError("unhandled accuracy case in Score.Hit()");
            break;
        }

        if(comboZone) {
            pointAdd *= comboMultiplier;
        }
        score += pointAdd;
        combo = Clamp(combo, 0, maxCombo);

        UI.UpdateScore(score, (float)combo/(float)maxCombo);

        if(combo >= maxCombo) {
            StartComboZone();
        }
    }

    public void FinishedDumpling() {
        dumplingsFinished++;
        UI.UpdateDumplings(dumplingsFinished);
    }

    public void EndGame() {
        UI.EndGame (
            score,
            longestStreak,
            dumplingsFinished,
            numPerf,
            numOk,
            numMiss
        );
    }

    private void StartComboZone () {
        comboTimeLeft = comboBonusTime;
        combo = 0;
        comboZone = true;
        UI.SetComboZoneVisible(true);
    }

    private void EndComboZone () {
        comboZone = false;
        UI.UpdateCombo(0);
        UI.SetComboZoneVisible(false);
    }

    private int Clamp(int x, int min, int max) {
        if (x < min) x = min;
        if (x > max) x = max;
        return x;
    }
}
