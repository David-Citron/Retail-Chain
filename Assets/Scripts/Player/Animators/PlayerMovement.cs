using UnityEngine;

public class PlayerMovement : MonoBehaviour
{

    public float speed = 4f;
    public float rotationSpeed = 8f;
    public float accelerationTime = 6.5f;
    private float currentSpeed = 0f;

    public GameObject currentPlatform;

    private Rigidbody rb;
    private Animator animator;
    private Vector3 movementInput;

    private Vector3 forward, right;
    private Vector3 moveDirection;
    private float horizontal, vertical;

    private bool walking;
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

    private void MovePlayer()
    {
        moveDirection = forward * vertical + right * horizontal;
        rb.AddForce(moveDirection.normalized * speed * 10f, ForceMode.Force);
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

    private Vector3 GetCameraRelativeDirection(float horizontal, float vertical)
    {
        // Get the camera's forward and right vectors
        Vector3 cameraForward = Camera.main.transform.forward;
        Vector3 cameraRight = Camera.main.transform.right;

        // Flatten the y-component to prevent vertical movement
        cameraForward.y = 0f;
        cameraRight.y = 0f;

        // Normalize the directions
        cameraForward.Normalize();
        cameraRight.Normalize();

        // Calculate movement direction relative to the camera
        return (cameraForward * vertical + cameraRight * horizontal);
    }

    /*
    void Update()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

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

        movementInput = new Vector3(horizontal, 0f, vertical).normalized;

        if (movementInput.magnitude >= 0.1f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(movementInput);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
    }

    void FixedUpdate()
    {
        if (movementInput.magnitude >= 0.1f)
        {
            currentSpeed = Mathf.MoveTowards(currentSpeed, speed, accelerationTime * Time.fixedDeltaTime); // Gradually increase speed

            Vector3 nextPosition = rb.position + movementInput * currentSpeed * Time.fixedDeltaTime;  // Calculate the next position


            GameObject hitObject;

            if (!IsObstructed(rb.position, nextPosition, out hitObject))
            {
                rb.AddForce(nextPosition, ForceMode.Force);
                return;
            }

            if (hitObject == null)
            {
                rb.AddForce(nextPosition, ForceMode.Force);
                return;
            }


            var rigidBody = hitObject.GetComponent<Rigidbody>();
            if (rigidBody != null && rigidBody.isKinematic)
            {
                currentSpeed = 0f;
                return;
            }

            rb.AddForce(nextPosition, ForceMode.Force);
        } else currentSpeed = 0f; // If no input, reset speed
    }

    bool IsObstructed(Vector3 currentPosition, Vector3 nextPosition, out GameObject hitObject)
    {
        float capsuleRadius = GetComponent<CapsuleCollider>().radius;
        float capsuleHeight = GetComponent<CapsuleCollider>().height / 2f;

        RaycastHit hit; // Struct to store hit information

        bool isBlocked = Physics.CapsuleCast(
            currentPosition + Vector3.up * capsuleHeight,
            currentPosition - Vector3.up * capsuleHeight,
            capsuleRadius,
            (nextPosition - currentPosition).normalized,
            out hit, // Store the hit result
            (nextPosition - currentPosition).magnitude,
            ~0, // Layer mask: Check all layers
            QueryTriggerInteraction.Ignore
        );

        if (isBlocked)
        {
            hitObject = hit.collider.gameObject; // Retrieve the hit object
        }
        else
        {
            hitObject = null; // No object was hit
        }

        return isBlocked;
    }*/
}