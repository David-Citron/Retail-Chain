using UnityEngine;

public class PlayerMovement : MonoBehaviour
{

    private CharacterController characterController;
    private Animator animator;

    public float moveSpeed = 5f;
    public float rotationSpeed = 5f;

    private bool walking;


    void Start()
    {
        characterController = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();

        for (int i = 0; i < transform.childCount; i++)
        {
            Transform childTransform = transform.GetChild(i);
            childTransform.rotation = Quaternion.Euler(new Vector3(childTransform.rotation.eulerAngles.x, 90, childTransform.rotation.eulerAngles.z));
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
        }
        else if (!walking && horizontal != 0 && vertical != 0)
        {
            animator.SetBool("walking", true);
            walking = true;
        }

        Vector3 moveDirection = new Vector3(horizontal, 0, vertical).normalized;
        if (moveDirection.magnitude >= 0.1f)
        {
            float targetAngle = Mathf.Atan2(moveDirection.x, moveDirection.z) * Mathf.Rad2Deg;

            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref rotationSpeed, 0.1f);
            transform.rotation = Quaternion.Euler(0f, angle, 0f);

            Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
            characterController.Move(moveDir.normalized * moveSpeed * Time.fixedDeltaTime);
        }
    }
}