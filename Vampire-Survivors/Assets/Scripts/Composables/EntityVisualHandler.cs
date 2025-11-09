using System.Collections;
using System.Threading.Tasks;
using UnityEngine;

public class EntityVisualHanlder : MonoBehaviour
{
    public SpriteRenderer SpriteRenderer { set; get; }
    public Animator Animator { set; get; }

    void Awake()
    {
        SpriteRenderer = GetComponent<SpriteRenderer>();
        Animator = GetComponent<Animator>();
    }

    public void FaceTarget(Transform target)
    {
        if (target != null)
        {
            SpriteRenderer.flipX = target.position.x < transform.position.x;
        }
    }


    // Note(Jovanni):
    // It is the job of the caller to do
    /*
        FadeOutTask().ContinueWith(() => 
            {
                // Use the correct one for your use case
                // GameManger.Instance.PlayerList.Remove(this.gameObject);
                // GameManger.Instance.EnemyList.Remove(this.gameObject);
                Destory(this.gameObject);
            }, 

            TaskScheduler.FromCurrentSynchronizationContext()
        );
    */
    public async Task FadeOutDeathTask(string deadAnimationName, bool fadeDuringAnimation)
    {
        Animator.Play(deadAnimationName);
        while (!Animator.GetCurrentAnimatorStateInfo(0).IsName(deadAnimationName))
        {
            await Task.Yield();
        }

        AnimatorStateInfo animInfo = Animator.GetCurrentAnimatorStateInfo(0);
        if (!fadeDuringAnimation)
        {
            await Task.Delay((int)(animInfo.length * 1000.0f));
        }


        const int steps = 100;
        float stepTime = animInfo.length / steps;
        Color color = SpriteRenderer.color;
        for (int i = 0; i < steps; i++)
        {
            color.a = 1f - (i / (float)steps);
            SpriteRenderer.color = color;
            await Task.Delay((int)(stepTime * 1000.0f));
        }
    }
}
