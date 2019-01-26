using UnityEngine;
using UnityEngine.UI;

public class PlayerInput : MonoBehaviour
{
    public TimingCounter TimingCounter;

    void Update () {
        if (Input.GetKeyDown("space"))
        {
            TimingCounter.NoteHit();
        }
    }
}
