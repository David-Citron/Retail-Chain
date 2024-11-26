using UnityEngine;

public class PlayerMovement : MonoBehaviour
{

    private CharacterController characterController;
    private Animator animator;

    public float moveSpeed = 3f;
    public float rotationSpeed = 5f;
    public Transform cameraTransform;


    private bool walking;


    void Start()
    {
        characterController = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();

        if (cameraTransform == null)
        {
            cameraTransform = Camera.main.transform;
        }
    }

    void FixedUpdate()
    {
        float horizontal = Input.GetAxis("Horizontal"); // A/D or Left/Right
        float vertical = Input.GetAxis("Vertical"); // W/S or Up/Down

        if (walking && horizontal == 0 && vertical == 0)
        {
            animator.SetBool("walking", false);
            walking = false;
        } else if(!walking && horizontal != 0 && vertical != 0) {
            animator.SetBool("walking", true);
            walking = true;
        }

        Vector3 moveDirection = new Vector3(horizontal, 0, vertical).normalized;

        if (moveDirection.magnitude >= 0.1f)
        {
            float targetAngle = Mathf.Atan2(moveDirection.x, moveDirection.z) * Mathf.Rad2Deg + cameraTransform.eulerAngles.y;

            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref rotationSpeed, 0.1f);
            transform.rotation = Quaternion.Euler(0f, angle, 0f);

            Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
            characterController.Move(moveDir.normalized * moveSpeed * Time.fixedDeltaTime);
        }
    }
}