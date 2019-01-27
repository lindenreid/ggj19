using UnityEngine;

public class DumplingAnimator : MonoBehaviour
{
    public Animator anim;
    public string animName;
    public int numFrames;

    private int frame;

    void Start()
    {
        anim.Play(animName, -1, 0.0f);
        anim.speed = 0.0f;

        frame = 0;
    }

    public void IncrementFrame() {
        frame++;
        if(frame > numFrames) {
            frame = 0;
        }

        anim.Play(animName, -1, (float)frame / (float)numFrames);
        anim.speed = 0.0f;
    }
}
