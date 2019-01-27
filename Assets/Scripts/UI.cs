using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI : MonoBehaviour
{
    public Notification missNotif;
    public Notification okNotif;
    public Notification perfNotif;
    public List<GameObject> comboObjs;
    public GameObject hudObj;

    public GameObject endgameObj;
    public Text finalScoreText;
    public Text streakText;
    public Text perfText;
    public Text okText;
    public Text missText;
    public Text dumplingText;

    public Text ScoreText;
    public Text DumplingText;
    public Renderer ComboBarRenderer;
    public string FillPercentPropertyName = "_FillPercent";

    private Material ComboMaterial;

    void Start() {
        ComboMaterial = ComboBarRenderer.material;
    }
    
    public void ShowAccuracy(Accuracy acc) {
        //Debug.Log("show accuracy: " + acc);
        switch(acc) {
            case Accuracy.Great:
                missNotif.Restart(enable:false);
                okNotif.Restart(enable:false);
                perfNotif.Restart(enable:true);
            break;
            case Accuracy.Ok:
                missNotif.Restart(enable:false);
                okNotif.Restart(enable:true);
                perfNotif.Restart(enable:false);
            break;
            case Accuracy.Miss:
                missNotif.Restart(enable:true);
                okNotif.Restart(enable:false);
                perfNotif.Restart(enable:false);
            break;
            default:
                Debug.LogError("accuracy case not handled!");
            break;
        }
    }

    public void UpdateScore(float score, float comboPercent) {
        ScoreText.text = "score: " + score;
        UpdateCombo(comboPercent);
    }

    public void UpdateCombo(float comboPercent) {
        ComboMaterial.SetFloat(FillPercentPropertyName, comboPercent);
    }

    public void UpdateDumplings(int dumplings) {
        DumplingText.text = "dumplings: " + dumplings;
    }

    public void SetComboZoneVisible (bool visible) {
        foreach(GameObject g in comboObjs) { 
            g.SetActive(visible);
        }
    }

    public void EndGame(
        float finalScore,
        int streak,
        int dumplings,
        int perf,
        int ok,
        int m
    ) {
        hudObj.SetActive(false);
        endgameObj.SetActive(true);

        finalScoreText.text = "final score: " + finalScore;
        streakText.text = "longest streak: " + streak;
        dumplingText.text = "dumplings made: " + dumplings;
        perfText.text = "" + perf;
        okText.text = "" + ok;
        missText.text = "" + m;
    }
}
