using UnityEngine;

public class SkeletonController : MonoBehaviour
{
    private Rigidbody2D rb;

    private PlayerController playerReference;
    private SpriteRenderer spriteRenderer;
    private Animator animator;


    // Attack cooldown thing and attack cone
    [SerializeField] private float attackRadius;
    // private readonly string ANIM_IDLE = "";
    // private readonly string ANIM_ATTACK_1 = "";

    void Awake()
    {
        this.playerReference = Object.FindAnyObjectByType<PlayerController>();
        this.spriteRenderer = GetComponent<SpriteRenderer>();
        this.animator = GetComponent<Animator>();

        
    }

    // Update is called once per frame
    void Update()
    {
        if (playerReference == null) return;

        this.spriteRenderer.flipX = playerReference.transform.position.x < this.transform.position.x;

        

        if (Vector3.Distance(this.playerReference.transform.position, this.transform.position) < this.attackRadius)
        {
            this.animator.SetTrigger("Attack1");
        }

        //this.rb.
       
    }
}
