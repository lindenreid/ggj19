using UnityEngine;
using UnityEngine.UI;

public class PlayerInput : MonoBehaviour
{
    public TimingCounter TimingCounter;
    public Text feedbackText;

    void Update () {
        if (Input.GetKeyDown("space"))
        {
            Accuracy acc = TimingCounter.GetAccuracy();
            Debug.Log("accuracy: " + acc);
            feedbackText.text = acc + "!";
        }
    }
}
