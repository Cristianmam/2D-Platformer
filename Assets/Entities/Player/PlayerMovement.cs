using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField]
    private float movementSpeed = 7;
    [SerializeField]
    private float maxStepUp = 0.25f;
    [SerializeField]
    private float stepSmooth = 0.1f;
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

    private bool launched;

    public float horizontalMovement;
    private float verticalMovement;

    private bool isGrounded;
    private bool isOnWall;

    public Rigidbody2D rBody { get; private set; }
    public BoxCollider2D bCol { get; private set; }



    [Header("Collisions")]
    [SerializeField]
    private PhysicsMaterial2D slipMaterial;
    [SerializeField]
    private PhysicsMaterial2D gripMaterial;

    private LayerMask groundLayer;
    private LayerMask wallLayer;
    private LayerMask oneWayPlatformLayer;

    [Header("Combat")]
    [SerializeField]
    private float knockBackForce = 750;
    [SerializeField]
    private float stunDuration = 0.5f;
    private float stunTimer = 0;
    private bool knockedBack = false;

    private Animator animator;

    private GameController gameController;

    private PlayerManager playerManager;

    public bool lowerHit;
    public bool upperHit;

    public void InitializeMovement(PlayerManager pm, GameController gc, Animator anim)
    {
        gameController = gc;
        animator = anim;
        playerManager = pm;

        rBody = GetComponent<Rigidbody2D>();
        bCol = GetComponent<BoxCollider2D>();

        groundLayer = gameController.groundLayer;
        wallLayer = gameController.wallLayer;
        oneWayPlatformLayer = gameController.oneWayPlatformLayer;
    }

    private void Update()
    {
        //Check if we are allowed to move
        if (!gameController.gameplayActive || playerManager.playerState != PlayerManager.PlayerStates.Normal)
            return;

        isGrounded = CheckGrounded();
        isOnWall = CheckOnWall();

        horizontalMovement = Input.GetAxis("Horizontal");
        verticalMovement = Input.GetAxis("Vertical");

        //We can check for things that should stop the player from altering the movement, like immediately after jumping off a wall
        if(wallJumpTimer <= 0 && stunTimer <= 0)
        {
            rBody.velocity = CalculatePlayerVelocity();

            if (knockedBack && !IsNearZero(horizontalMovement) || isGrounded)
                knockedBack = false;
            
            if(!knockedBack)
                if (rBody.velocity.x > 0.01f)
                    transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y,transform.localScale.z);
                else if (rBody.velocity.x < -0.01f)
                    transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }
  

        if (isGrounded)
            rBody.sharedMaterial = gripMaterial;
        else
            rBody.sharedMaterial = slipMaterial;

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

        if (Input.GetKeyDown(KeyCode.Space) && !(Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow)))
        {
            Jump();
        }

        if (!IsNearZero(horizontalMovement))
            StepUp();

        NotifyAnimator();

        if (wallJumpTimer > 0)
            wallJumpTimer -= Time.deltaTime;

        if (stunTimer > 0)
            stunTimer -= Time.deltaTime;
    }

    public void KnockPlayerBack(Vector3 knockBackOrigin)
    {
        if (knockedBack)
            return;

        stunTimer = stunDuration;
        knockedBack = true;

        Vector2 playerCenter = new Vector2(transform.position.x, transform.position.y + bCol.size.y / 2);
        Vector2 origin = new Vector2(knockBackOrigin.x, knockBackOrigin.y);

        Vector2 resultant = playerCenter - origin;

        resultant = resultant.normalized;

        resultant = resultant * knockBackForce;

        rBody.AddForce(resultant);
    }


    private void Jump()
    {
        if (isOnWall && wallJumpTimer <= 0 && !isGrounded)
        {
            rBody.velocity = new Vector2(-Mathf.Sign(transform.localScale.x) * wallJumpHorizontalForce, wallJumpVerticalForce);


            wallJumpTimer = wallJumpCooldown;
            transform.localScale = new Vector3(-Mathf.Sign(transform.localScale.x), transform.localScale.y, transform.localScale.z);

            launched = true;
        }

        if (isGrounded && !(CheckOnOneWayPlatform() && Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow)))
            rBody.velocity = new Vector2(rBody.velocity.x, jumpHeight);
    }

    private bool CheckGrounded()
    {
        if (Physics2D.BoxCast(bCol.bounds.center, bCol.bounds.size, 0, Vector2.down, 0.1f, groundLayer) || CheckOnOneWayPlatform())
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private bool CheckOnOneWayPlatform()
    {
        if (Physics2D.BoxCast(bCol.bounds.center, bCol.bounds.size, 0, Vector2.down, 0.1f, oneWayPlatformLayer) && IsNearZero(rBody.velocity.y))
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
        animator.SetBool("Grounded", isGrounded);

        animator.SetBool("Running", !IsNearZero(horizontalMovement));

        animator.SetBool("OnWall", isOnWall);

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
     
    }

    private Vector2 CalculatePlayerVelocity()
    {
        Vector2 finalVelocity;

        //Allows the player to keep momentum when launched, like when jumping off a wall
        if (!isGrounded && launched)
        {
            //If we move in the same direction of the jump, keep the momentum, otherwise kill it
            if (Mathf.Sign(rBody.velocity.x) == Mathf.Sign(horizontalMovement) || IsNearZero(horizontalMovement))
            {
                finalVelocity = new Vector2(Mathf.Clamp(rBody.velocity.x + horizontalMovement * movementSpeed, -movementSpeed, movementSpeed), rBody.velocity.y);
                return finalVelocity;
            }
            else
                launched = false;
        }

        finalVelocity = new Vector2(horizontalMovement * movementSpeed, rBody.velocity.y);
        return finalVelocity;
    }

    private bool IsNearZero(float f)
    {
        return IsNearZero(f, 0.01f);
    }

    private bool IsNearZero(float f, float range)
    {
        if (f > -range && f < range)
            return true;
        else
            return false;
    }

    private void StepUp()
    {
        float vectorLength = bCol.bounds.size.x / 2 + 0.03f;
        Vector2 lowerCastPoint = new Vector2(bCol.bounds.center.x, bCol.bounds.center.y - bCol.bounds.size.y / 2);
        Vector2 directionVector = new Vector2(Mathf.Sign(horizontalMovement), 0);
        if (Physics2D.Raycast(lowerCastPoint, directionVector, vectorLength, groundLayer))
        {
            Vector2 upperCastPoint = new Vector2(lowerCastPoint.x, lowerCastPoint.y + maxStepUp);
            if (!Physics2D.Raycast(upperCastPoint, directionVector, vectorLength, groundLayer))
            {
                rBody.position -= new Vector2(0, -stepSmooth);
            }
        }
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
