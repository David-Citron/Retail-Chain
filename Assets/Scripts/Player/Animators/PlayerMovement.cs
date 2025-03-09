using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float walkSpeed = 1.0f;
    [SerializeField] private float sprintSpeed = 1.5f;

    public float speed = 4f;
    public float rotationSpeed = 8f;

    private Rigidbody rb;
    private Animator animator;
    private Vector3 movementInput;

    private Vector3 moveDirection;
    private float horizontal, vertical;

    private bool walking;

    private Vector3 lastWallNormal;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();

        rb.freezeRotation = true;

        //Rotate player 90? on Y axis to face correct direction
        for (int i = 0; i < transform.childCount; i++)
        {
            Transform childTransform = transform.GetChild(i);
            childTransform.rotation = Quaternion.Euler(new Vector3(childTransform.rotation.eulerAngles.x, 90, childTransform.rotation.eulerAngles.z));
        }
    }

    void Update()
    {
        horizontal = KeybindManager.instance.keybinds[ActionType.HorizontalInput].CalculateAxis();
        vertical = KeybindManager.instance.keybinds[ActionType.VerticalInput].CalculateAxis();
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

        if (Input.GetKey(KeyCode.LeftShift)) speed = sprintSpeed;
        else speed = walkSpeed;

        MovePlayer();
        RotatePlayer();
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
        Vector3 inputDirection = new Vector3(horizontal, 0f, vertical).normalized;

        // If hitting a wall, project movement along the wall's plane
        if (lastWallNormal != Vector3.zero)
        {
            Vector3 projectedDirection = Vector3.ProjectOnPlane(inputDirection, lastWallNormal).normalized;

            // Allow movement away from the wall
            float dot = Vector3.Dot(inputDirection, lastWallNormal);
            if (dot > 0) inputDirection = inputDirection.normalized;
            else inputDirection = projectedDirection;
        }

        moveDirection = inputDirection;
        rb.AddForce(moveDirection.normalized * speed, ForceMode.VelocityChange);
    }

    private void RotatePlayer()
    {
        movementInput = new Vector3(horizontal, 0f, vertical).normalized;

        if (movementInput.magnitude >= 0.1f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(movementInput);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.fixedDeltaTime);
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