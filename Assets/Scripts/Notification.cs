using UnityEngine;

public class Notification : MonoBehaviour
{
    public Vector3 originalLocation;
    public Vector3 anim;
    public float duration;

    private float timeRemaining;
    private bool animating = false;

    void Start () {
    }

    void Update () {
        if(animating) {
            if(timeRemaining > 0.0f) {
                transform.localPosition += anim * Time.deltaTime;
            } else {
                Restart(enable:false);
            }
            timeRemaining -= Time.deltaTime;
        }
    }

    public void Restart(bool enable) {
        transform.localPosition = originalLocation;
        timeRemaining = duration;
        gameObject.SetActive(enable);
        animating = enable;
    }
}
