using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField]
    private float movementSpeed;
    [SerializeField]
    private float jumpHeight;

    private bool isGrounded;

    private Rigidbody2D rBody;
    private BoxCollider2D bCol;

    [Header("Visuals")]
    [SerializeField]
    private LayerMask groundLayer;

    private Animator animator;

    [Header("Camera positions")]
    [SerializeField]
    private float lookTimer;
    private float timeLooking;
    [SerializeField]
    private GameObject forwardCameraAnchor;
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
        float horizontalMovement = Input.GetAxis("Horizontal");

        rBody.velocity = new Vector2(horizontalMovement * movementSpeed, rBody.velocity.y);

        if (horizontalMovement > 0.01f)
            transform.localScale = Vector3.one;
        else if (horizontalMovement < -0.01f)
            transform.localScale = new Vector3(-1, 1, 1);
        

        isGrounded = CheckGrounded();

        if (Input.GetKey(KeyCode.Space))
        {
            Jump();
        }

        MoveCamera();

        if(isGrounded)
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

    }


    private void Jump()
    {
        if(isGrounded)
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

    private void MoveCamera()
    {
        float verticalMovement = Input.GetAxis("Vertical");

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
}
