using UnityEngine;
using UnityEngine.UI;

public class UI : MonoBehaviour
{
    public GameObject missObj;
    public GameObject okObj;
    public GameObject perfObj;

    public Text ScoreText;
    
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

    public void UpdateScore(float score) {
        ScoreText.text = "" + score;
    }
}
