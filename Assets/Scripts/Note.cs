using UnityEngine;

public class Note : MonoBehaviour
{
    public Vector3 animGrowth;
    public float animDuration;

    private float speed;
    private float dest; //x-pos of destination
    private float distancePastGoal; //distance (on x axis) that note is allowed to scroll past goal before being dying
    private TimingCounter TimingCounter;

    private bool animating;
    private float animTime;

    void Update () {
        transform.Translate(new Vector3(-speed, 0, 0) * Time.deltaTime);
        if(transform.position.x <= dest - distancePastGoal) {
            TimingCounter.NoteDied();
            Destroy(gameObject);
        }

        if(animating) {
            if(animTime <= animDuration/2.0f) {
                transform.localScale += animGrowth * Time.deltaTime;
            } else {
                transform.localScale -= animGrowth * Time.deltaTime;
            }

            animTime += Time.deltaTime;
            if(animTime >= animDuration) {
                animating = false;
            }
        }
    }

    public void Init(float speed, float dest, float past, TimingCounter timingCounter) {
        this.speed = speed;
        this.dest = dest;
        distancePastGoal = past;
        this.TimingCounter = timingCounter;
    }

    public void PlayHitAnimation () {
        animating = true;
        animTime = 0.0f;
    } 

}