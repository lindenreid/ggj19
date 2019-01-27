using UnityEngine;

public class Notification : MonoBehaviour
{
    public Vector3 originalLocation;
    public Vector3 anim;
    public float duration;

    private float timeRemaining;

    void Start () {
        Restart(enable:false);
    }

    void Update () {
        timeRemaining -= Time.deltaTime;
        if(timeRemaining > 0.0f) {
            transform.localPosition += anim * Time.deltaTime;
        } else {
            Restart(enable:false);
        }
    }

    public void Restart(bool enable) {
        transform.localPosition = originalLocation;
        timeRemaining = duration;
        gameObject.SetActive(enable);
    }
}
