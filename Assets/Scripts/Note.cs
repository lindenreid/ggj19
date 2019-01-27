using UnityEngine;

public class Note : MonoBehaviour
{
    public SpriteRenderer SpriteRenderer;
    public Vector3 animDir;
    public float animDuration;
    public float fadeSpeed;
    public Sprite UpSprite;
    public Sprite DownSprite;
    public Sprite LeftSprite;
    public Sprite RightSprite;

    private float beat;
    private float speed;
    private float dest; //x-pos of destination
    private float distancePastGoal; //distance (on x axis) that note is allowed to scroll past goal before being dying
    private TimingCounter TimingCounter;

    private bool dying;
    private float animTime;

    void Update () {
        if(dying) {
            transform.position += animDir * Time.deltaTime;
            float a = SpriteRenderer.color.a - fadeSpeed * Time.deltaTime;
            SpriteRenderer.color = new Color(
                SpriteRenderer.color.r,
                SpriteRenderer.color.b,
                SpriteRenderer.color.g,
                a
            );

            animTime += Time.deltaTime;
            if(animTime >= animDuration) {
                KillNote();
            }
        } else {
            transform.Translate(new Vector3(-speed, 0, 0) * Time.deltaTime);
            if(transform.position.x <= dest - distancePastGoal) {
                TimingCounter.NoteDied();
                KillNote();
            }
        }
    }

    public void Init( 
        float beat,
        float speed,
        float dest,
        float past,
        BeatType beatType,
        TimingCounter timingCounter
    ){
        this.beat = beat;
        this.speed = speed;
        this.dest = dest;
        distancePastGoal = past;
        this.TimingCounter = timingCounter;

        switch(beatType) {
            case BeatType.Up :
                SpriteRenderer.sprite = UpSprite;
            break;
            case BeatType.Down :
                SpriteRenderer.sprite = DownSprite;
            break;
            case BeatType.Left :
                SpriteRenderer.sprite = LeftSprite;
            break;
            case BeatType.Right :
                SpriteRenderer.sprite = RightSprite;
            break;
            default:
                Debug.LogError("unhandled beat type case");
            break;
        }
    }

    public void PlayHitAnimation () {
        if(!dying) {
            dying = true;
            animTime = 0.0f;
        }
    } 

    private void KillNote() {
        TimingCounter.NoteController.NoteDied(beat);
        Destroy(gameObject);
    }

}