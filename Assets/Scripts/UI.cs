using UnityEngine;
using UnityEngine.UI;

public class UI : MonoBehaviour
{
    public GameObject missObj;
    public GameObject okObj;
    public GameObject perfObj;
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
                missObj.SetActive(false);
                okObj.SetActive(false);
                perfObj.SetActive(true);
            break;
            case Accuracy.Ok:
                missObj.SetActive(false);
                okObj.SetActive(true);
                perfObj.SetActive(false);
            break;
            case Accuracy.Miss:
                missObj.SetActive(true);
                okObj.SetActive(false);
                perfObj.SetActive(false);
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
