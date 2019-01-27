using UnityEngine;

public class Score : MonoBehaviour
{
    public UI UI;
    
    public float pointsPerOk = 1.0f;
    public float pointsPerPerfect = 1.5f;
    public float pointsLostPerMiss = 1.0f;

    private float score;
    private int streak;

    void Start () {
        Init();
    }

    public void Init () {
        score = 0.0f;
        streak = 0;
    }

    public void NoteHit (Accuracy acc) {
        switch(acc) {
            case Accuracy.Miss:
                streak = 0;
            break;
            case Accuracy.Ok:
                score += pointsPerOk;
                streak ++;
            break;
            case Accuracy.Great:
                score += pointsPerPerfect;
                streak ++;
            break;
            default:
                Debug.LogError("unhandled accuracy case in Score.Hit()");
            break;
        }
        UI.UpdateScore(score);
    }
}
