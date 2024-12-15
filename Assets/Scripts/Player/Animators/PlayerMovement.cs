using UnityEngine;

public class PlayerMovement : MonoBehaviour
{

    public float speed = 4f;
    public float rotationSpeed = 8f;

    private Rigidbody rb;
    private Animator animator;
    private Vector3 movementInput;

    private Vector3 forward, right;
    private Vector3 moveDirection;
    private float horizontal, vertical;

    private bool walking;


    public Vector3 lastWallNormal;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();

        rb.freezeRotation = true;

        forward = transform.forward;
        right = transform.right;

        //Rotate player 90? on Y axis to face correct direction
        for (int i = 0; i < transform.childCount; i++)
        {
            Transform childTransform = transform.GetChild(i);
            childTransform.rotation = Quaternion.Euler(new Vector3(childTransform.rotation.eulerAngles.x, 90, childTransform.rotation.eulerAngles.z));
        }
    }

    void Update()
    {
        horizontal = Input.GetAxis("Horizontal");
        vertical = Input.GetAxis("Vertical");

        RotatePlayer();
    }

    void FixedUpdate()
    {
        if (walking && horizontal == 0 && vertical == 0)
        {
            animator.SetBool("walking", false);
            walking = false;
            return;
        }
        else if (!walking && (horizontal != 0 || vertical != 0))
        {
            animator.SetBool("walking", true);
            walking = true;
        }

        MovePlayer();
    }

    void LateUpdate()
    {
        if (rb.velocity.z == 0)
        {
            rb.velocity = Vector3.zero;
        }
    }

    private void MovePlayer()
    {
        //         moveDirection = forward * vertical + right * horizontal;
        //         rb.AddForce(moveDirection.normalized * speed, ForceMode.VelocityChange);

        // Base movement input
        Vector3 inputDirection = new Vector3(horizontal, 0f, vertical).normalized;

        // If hitting a wall, project movement along the wall's plane
        if (lastWallNormal != Vector3.zero)
        {
            inputDirection = Vector3.ProjectOnPlane(inputDirection, lastWallNormal).normalized;
        }

        moveDirection = inputDirection * speed;
        rb.AddForce(moveDirection.normalized, ForceMode.VelocityChange);
    }

    private void RotatePlayer()
    {
        movementInput = new Vector3(horizontal, 0f, vertical).normalized;

        if (movementInput.magnitude >= 0.1f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(movementInput);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
    }

    // Track wall collisions and their normals
    private void OnCollisionStay(Collision collision)
    {
        if(collision.rigidbody != null && !collision.rigidbody.isKinematic) return;

        foreach (ContactPoint contact in collision.contacts)
        {
            if (Vector3.Dot(contact.normal, Vector3.up) < 0.5f)
            {
                lastWallNormal = contact.normal;
                return;
            } else lastWallNormal = Vector3.zero;
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.rigidbody != null && !collision.rigidbody.isKinematic) return;

        // Reset wall normal when no longer colliding
        lastWallNormal = Vector3.zero;
    }
}