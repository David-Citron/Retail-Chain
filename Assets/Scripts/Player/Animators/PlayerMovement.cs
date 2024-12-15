using UnityEngine;

public class PlayerMovement : MonoBehaviour
{

    public float speed = 1f;
    public float rotationSpeed = 1f;
    public float accelerationTime = 0.5f;
    private float currentSpeed = 0f;

    public GameObject currentPlatform;

    private Rigidbody rb;
    private Animator animator;
    private Vector3 movementInput;

    private bool walking;


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
                rb.MovePosition(nextPosition);
                return;
            } 

            if (hitObject == null)
            {
                rb.MovePosition(nextPosition);
                return;
            }


            var rigidBody = hitObject.GetComponent<Rigidbody>();
            if (rigidBody != null && rigidBody.isKinematic)
            {
                currentSpeed = 0f;
                return;
            }

            rb.MovePosition(nextPosition);
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
    }
}