using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI : MonoBehaviour
{
    public Notification missNotif;
    public Notification okNotif;
    public Notification perfNotif;
    public List<GameObject> comboObjs;

    public Text ScoreText;
    public Renderer ComboBarRenderer;
    public string FillPercentPropertyName = "_FillPercent";

    private Material ComboMaterial;

    void Start() {
        ComboMaterial = ComboBarRenderer.material;
    }
    
    public void ShowAccuracy(Accuracy acc) {
        Debug.Log("show accuracy: " + acc);
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
        ScoreText.text = "" + score;
        UpdateCombo(comboPercent);
    }

    public void UpdateCombo(float comboPercent) {
        ComboMaterial.SetFloat(FillPercentPropertyName, comboPercent);
    }

    public void SetComboZoneVisible (bool visible) {
        foreach(GameObject g in comboObjs) { 
            g.SetActive(visible);
        }
    }
}
