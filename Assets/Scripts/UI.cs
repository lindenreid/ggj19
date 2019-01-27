using UnityEngine;
using UnityEngine.UI;

public class UI : MonoBehaviour
{
    public Notification missNotif;
    public Notification okNotif;
    public Notification perfNotif;
    public GameObject comboObj;

    public Text ScoreText;
    public Renderer ComboBarRenderer;
    public string FillPercentPropertyName = "_FillPercent";

    private Material ComboMaterial;

    void Start() {
        ComboMaterial = ComboBarRenderer.material;
    }
    
    public void ShowAccuracy(Accuracy acc) {
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

    public void ShowComboZone () {
        comboObj.SetActive(true);
    }

    public void HideComboZone () {
        comboObj.SetActive(false);
    }
}
