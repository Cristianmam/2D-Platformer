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

    [Header("Camera positions")]
    [SerializeField]
    private float lookTimer;
    private float timeLooking;
    [SerializeField]
    private GameObject forwardCameraAnchor;
    [SerializeField]
    private GameObject backwardCameraAnchor;
    [SerializeField]
    private GameObject topCameraAnchor;
    [SerializeField]
    private GameObject bottomCameraAnchor;

    private CameraControl mainCameraControl;

    private void Awake()
    {
        rBody = GetComponent<Rigidbody2D>();
        bCol = GetComponent<BoxCollider2D>();
        animator = GetComponent<Animator>();

        mainCameraControl = Camera.main.GetComponent<CameraControl>();
    }

    private void Update()
    {
        horizontalMovement = Input.GetAxis("Horizontal");

        if(wallJumpTimer <= 0)
            rBody.velocity = new Vector2(horizontalMovement * movementSpeed, rBody.velocity.y);

        if (horizontalMovement > 0.01f)
            transform.localScale = Vector3.one;
        else if (horizontalMovement < -0.01f)
            transform.localScale = new Vector3(-1, 1, 1);
        

        if (CheckOnWall())
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

        MoveCamera();

        NotifyAnimator();

        AdvanceTimers();
    }


    private void Jump()
    {
        if(CheckGrounded())
            rBody.velocity = new Vector2(rBody.velocity.x, jumpHeight);

        if (CheckOnWall() && wallJumpTimer <= 0 && !CheckGrounded())
        {
            if (horizontalMovement != 0)
                rBody.velocity =new Vector2(-Mathf.Sign(transform.localScale.x) * wallJumpHorizontalForce, wallJumpVerticalForce);
            else
                rBody.velocity = new Vector2(-Mathf.Sign(transform.localScale.x) * wallJumpHorizontalForce * 2, wallJumpVerticalForce);

            wallJumpTimer = wallJumpCooldown;
            transform.localScale = new Vector3(-Mathf.Sign(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }   
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
    }

    private void MoveCamera()
    {
        float verticalMovement = Input.GetAxis("Vertical");

        if (CheckOnWall())
        {
            timeLooking = 0;
            mainCameraControl.LerpLookAt(backwardCameraAnchor.transform.position);
            return;
        }

        if (rBody.velocity.magnitude > 0.1f)
        {
            timeLooking = 0;
            mainCameraControl.LerpLookAt(forwardCameraAnchor.transform.position);
            return;
        }

        if(verticalMovement == 0)
        {
            timeLooking = 0;
            mainCameraControl.LerpLookAt(forwardCameraAnchor.transform.position);
            return;
        }


        if (timeLooking < lookTimer)
            timeLooking += Time.deltaTime;
        else
        {
            if (verticalMovement > 0)
                mainCameraControl.LerpLookAt(topCameraAnchor.transform.position);
            else
                mainCameraControl.LerpLookAt(bottomCameraAnchor.transform.position);
        }
    }

    private void AdvanceTimers()
    {
        if (wallJumpTimer > 0)
            wallJumpTimer -= Time.deltaTime;
    }
}
