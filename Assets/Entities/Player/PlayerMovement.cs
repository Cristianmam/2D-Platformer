using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField]
    private float movementSpeed = 10;
    [SerializeField]
    private float jumpHeight = 20;
    [SerializeField]
    private float wallJumpCooldown = 0.5f;
    private float wallJumpTimer = 0;
    [SerializeField]
    private float wallJumpHorizontalForce = 1250;
    [SerializeField]
    private float wallJumpVerticalForce = 750;
    [SerializeField]
    private float baseGravityScale = 5;
    [SerializeField]
    private float wallHangGravityScale = 1;

    private float horizontalMovement;

    private Rigidbody2D rBody;
    private BoxCollider2D bCol;

    [Header("Collisions")]
    [SerializeField]
    private LayerMask groundLayer;
    [SerializeField]
    private LayerMask wallLayer;

    private Animator animator;

    private GameController gameController;

    private PlayerManager playerManager;


    private void Awake()
    {
        rBody = GetComponent<Rigidbody2D>();
        bCol = GetComponent<BoxCollider2D>();
    }

    public void InitializeMovement(PlayerManager pm, GameController gc, Animator anim)
    {
        gameController = gc;
        animator = anim;
        playerManager = pm;
    }

    private void Update()
    {
        //Check if we are allowed to move
        if (!gameController.gameplayActive || !playerManager.takingInputs)
            return;

        horizontalMovement = Input.GetAxis("Horizontal");

        if(wallJumpTimer <= 0)
        {
            rBody.velocity = new Vector2(horizontalMovement * movementSpeed, rBody.velocity.y);

            if (horizontalMovement > 0.01f)
                transform.localScale = Vector3.one;
            else if (horizontalMovement < -0.01f)
                transform.localScale = new Vector3(-1, 1, 1);
        }
            
        

        if (CheckOnWall() && !CheckGrounded() && rBody.velocity.y < 0.01f)
        {
            rBody.gravityScale = wallHangGravityScale;
        }
        else
        {
            rBody.gravityScale = baseGravityScale;
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            Jump();
        }

        NotifyAnimator();

        AdvanceTimers();
    }


    private void Jump()
    {
        if (CheckOnWall() && wallJumpTimer <= 0 && !CheckGrounded())
        {
            if (horizontalMovement != 0)
                rBody.velocity = new Vector2(-Mathf.Sign(transform.localScale.x) * wallJumpHorizontalForce, wallJumpVerticalForce);
            else
                rBody.velocity = new Vector2(-Mathf.Sign(transform.localScale.x) * wallJumpHorizontalForce * 2, wallJumpVerticalForce);

            wallJumpTimer = wallJumpCooldown;
            transform.localScale = new Vector3(-Mathf.Sign(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }

        if (CheckGrounded())
            rBody.velocity = new Vector2(rBody.velocity.x, jumpHeight);
    }

    private bool CheckGrounded()
    {
        if (Physics2D.BoxCast(bCol.bounds.center, bCol.bounds.size, 0, Vector2.down, 0.1f, groundLayer))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private bool CheckOnWall()
    {
        if(Physics2D.BoxCast(bCol.bounds.center, bCol.bounds.size,0,new Vector2(transform.localScale.x, 0), 0.1f, wallLayer) && !CheckGrounded())
        {
            return true;
        }
        else
        {
            return false;
        }  
    }

    private void NotifyAnimator()
    {
        if (CheckGrounded())
            animator.SetBool("Grounded", true);
        else
            animator.SetBool("Grounded", false);

        if (rBody.velocity.x > -0.01f && rBody.velocity.x < 0.01f)
            animator.SetBool("Running", false);
        else
            animator.SetBool("Running", true);

        if (rBody.velocity.y > 0.01f)
            animator.SetBool("Jumping", true);
        else
            animator.SetBool("Jumping", false);

        if (rBody.velocity.y > 0.01f && rBody.velocity.y < 0.5f)
            animator.SetBool("Cusping", true);
        else
            animator.SetBool("Cusping", false);

        if (!CheckGrounded() && rBody.velocity.y < -0.01f)
            animator.SetBool("Falling", true);
        else
            animator.SetBool("Falling", false);

        if (CheckOnWall())
            animator.SetBool("OnWall", true);
        else
            animator.SetBool("OnWall", false);
    }

    private void AdvanceTimers()
    {
        if (wallJumpTimer > 0)
            wallJumpTimer -= Time.deltaTime;
    }
}
