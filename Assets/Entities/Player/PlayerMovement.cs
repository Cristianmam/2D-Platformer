using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField]
    private float movementSpeed = 7;
    [SerializeField]
    private float jumpHeight = 15;
    [SerializeField]
    private float wallJumpCooldown = 0.5f;
    private float wallJumpTimer = 0;
    [SerializeField]
    private float wallJumpHorizontalForce = 1250;
    [SerializeField]
    private float wallJumpVerticalForce = 750;
    [SerializeField]
    private float baseGravityScale = 3;
    [SerializeField]
    private float wallHangGravityScale = 0.25f;

    public float horizontalMovement;
    public float verticalMovement;

    private bool isGrounded;
    private bool isOnWall;

    private Rigidbody2D rBody;
    private BoxCollider2D bCol;

    [Header("Collisions")]
    [SerializeField]
    private LayerMask groundLayer;
    [SerializeField]
    private LayerMask wallLayer;

    [SerializeField]
    private PhysicsMaterial2D slipMaterial;
    [SerializeField]
    private PhysicsMaterial2D gripMaterial;


    private Animator animator;

    private GameController gameController;

    private PlayerManager playerManager;


    public void InitializeMovement(PlayerManager pm, GameController gc, Animator anim)
    {
        gameController = gc;
        animator = anim;
        playerManager = pm;

        rBody = GetComponent<Rigidbody2D>();
        bCol = GetComponent<BoxCollider2D>();
    }

    private void Update()
    {
        //Check if we are allowed to move
        if (!gameController.gameplayActive)
            return;

        isGrounded = CheckGrounded();
        isOnWall = CheckOnWall();

        horizontalMovement = Input.GetAxis("Horizontal");
        verticalMovement = Input.GetAxis("Vertical");

        //We can check for things that should stop the player from altering the movement, like immediately after jumping off a wall
        if(wallJumpTimer <= 0)
        {
            rBody.velocity = CalculatePlayerVelocity();

            if (horizontalMovement > 0.01f)
                transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y,transform.localScale.z);
            else if (horizontalMovement < -0.01f)
                transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }

        
        if(verticalMovement < -0.01f)
        {
            if(!isOnWall && !isGrounded)
                rBody.velocity = new Vector2(0, rBody.velocity.y);
        }

        if (isOnWall)
            rBody.sharedMaterial = slipMaterial;
        else
            rBody.sharedMaterial = gripMaterial;

        //Wall hanging
        if (isOnWall && !isGrounded && rBody.velocity.y < 0.01f)
        {
            //If we move towards a wall, stop falling
            if (MovingInSameDirectionAsWall())
            {
                rBody.gravityScale = 0;
                //Even tho gravity scale is set to 0 the character still falls, which led to this fix
                rBody.velocity = new Vector2(rBody.velocity.x, 0);
            }
            //If not fall slowly
            else
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

        if (wallJumpTimer > 0)
            wallJumpTimer -= Time.deltaTime;
    }


    private void Jump()
    {
        if (isOnWall && wallJumpTimer <= 0 && !isGrounded)
        {
            rBody.velocity = new Vector2(-Mathf.Sign(transform.localScale.x) * wallJumpHorizontalForce, wallJumpVerticalForce);


            wallJumpTimer = wallJumpCooldown;
            transform.localScale = new Vector3(-Mathf.Sign(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }

        if (isGrounded)
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
        if(Physics2D.BoxCast(bCol.bounds.center, bCol.bounds.size,0,new Vector2(transform.localScale.x, 0), 0.1f, wallLayer) && !isGrounded)
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
        if (isGrounded)
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

        if (!isGrounded && rBody.velocity.y < -0.01f)
            animator.SetBool("Falling", true);
        else
            animator.SetBool("Falling", false);

        if (isOnWall)
            animator.SetBool("OnWall", true);
        else
            animator.SetBool("OnWall", false);
    }

    private Vector2 CalculatePlayerVelocity()
    {
        Vector2 finalVelocity;

        //Allows the player to keep momentum while airborne, like when jumping off a wall
        if (!isGrounded)
        {
            //If we move in the same direction of the jump, keep the momentum, otherwise kill it
            if(Mathf.Sign(rBody.velocity.x) == Mathf.Sign(horizontalMovement) || horizontalMovement > -0.01f && horizontalMovement < 0.01f)
            {
                finalVelocity = new Vector2(Mathf.Clamp(rBody.velocity.x + horizontalMovement * movementSpeed, -movementSpeed, movementSpeed), rBody.velocity.y);
                return finalVelocity;
            }
        }

        finalVelocity = new Vector2(horizontalMovement * movementSpeed, rBody.velocity.y);
        return finalVelocity;
    }

    private bool MovingInSameDirectionAsWall()
    {
        if (isOnWall)
            if (horizontalMovement != 0)
                if (Mathf.Sign(horizontalMovement) == Mathf.Sign(transform.localScale.x))
                    return true;

        return false;
    }
}
