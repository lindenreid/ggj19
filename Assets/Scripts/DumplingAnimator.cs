using UnityEngine;

public class DumplingAnimator : MonoBehaviour
{
    public Score Score;
    public SpriteRenderer SpriteRenderer;
    public Animator anim;
    public string animName;
    public int numFrames;
    public Vector3 originalLocation;
    public Vector3 finishAnimation;
    public float fadeSpeed;
    public float finishAnimDuration;

    private int frame;
    private float animLeft;
    private bool playingFinishAnimation;

    void Start()
    {
        anim.Play(animName, -1, 0.0f);
        anim.speed = 0.0f;

        frame = 0;
        animLeft = 0.0f;
    }

    void Update () {
        if(playingFinishAnimation) {
            animLeft -= Time.deltaTime;
            transform.localPosition += finishAnimation * Time.deltaTime;
            float a = SpriteRenderer.color.a - fadeSpeed * Time.deltaTime;
            SpriteRenderer.color = new Color(
                SpriteRenderer.color.r,
                SpriteRenderer.color.b,
                SpriteRenderer.color.g,
                a
            );
            if(animLeft <= 0.0f) {
                EndAnimation();
                frame = 0;
            }       
        }
    }

    public void IncrementFrame() {
        if(!playingFinishAnimation) {
            frame++;
        }
        else {
            EndAnimation();
            frame = 0;
        }

        if(frame == numFrames) {
            StartAnimation();
            anim.Play(animName, -1, 0.9f);
        } else {
            anim.Play(animName, -1, (float)frame / (float)numFrames);
        }

        anim.speed = 0.0f;
    }

    private void EndAnimation () {
        playingFinishAnimation = false;
        transform.localPosition = originalLocation;
        anim.Play(animName, -1, 0);
        anim.speed = 0.0f;

        SpriteRenderer.color = new Color(
            SpriteRenderer.color.r,
            SpriteRenderer.color.b,
            SpriteRenderer.color.g,
            1.0f
        );

        Score.FinishedDumpling();
    }

    private void StartAnimation () {
        animLeft = finishAnimDuration;
        playingFinishAnimation = true;
        //transform.localPosition += finishAnimation * Time.deltaTime;
        //animLeft -= Time.deltaTime;
    }
}
