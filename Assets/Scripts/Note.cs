using UnityEngine;

public class Note : MonoBehaviour
{
    private float speed;
    private float dest;
    private float distancePastGoal;

    public void Init(float speed, float dest, float past) {
        this.speed = speed;
        this.dest = dest;
        distancePastGoal = past;
    } 

    void Update () {
        transform.Translate(new Vector3(-speed, 0, 0) * Time.deltaTime);
        if(transform.position.x <= dest - distancePastGoal) {
            Destroy(gameObject);
        }
    }

}