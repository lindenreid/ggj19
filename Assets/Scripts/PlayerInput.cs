using UnityEngine;
using UnityEngine.UI;

public class PlayerInput : MonoBehaviour
{
    public TimingCounter TimingCounter;

    void Update () {
        // LLOK, THERES PROBABLY A BETTER WAY TO DO THIS
        if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W))
        {
            TimingCounter.NoteHit(BeatType.Up);
        } else if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S))
        {
            TimingCounter.NoteHit(BeatType.Down);
        } else if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A))
        {
            TimingCounter.NoteHit(BeatType.Left);
        } else if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D))
        {
            TimingCounter.NoteHit(BeatType.Right);
        }
    }
}
