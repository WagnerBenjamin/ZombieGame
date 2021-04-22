using Mirror;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovementScript : NetworkBehaviour
{
    [SerializeField]
    private Camera cam;
    [SerializeField]
    private Transform groundCheck;
    [SerializeField]
    LayerMask groundMask;

    private CharacterController CController;
    private Vector3 velocity;
    private float rotation;
    private float camRotation;
    private bool isGrounded;
    float groundDistance = 0.2f;
    private float gravity = -9.8f;
    private Vector3 yVelocity;
    private float jumpHeight = 1.5f;
    private float sprintMultiplier = 1.5f;
    private bool isSprinting;

    // Start is called before the first frame update
    void Start()
    {
        CController = GetComponent<CharacterController>();
    }

    private void FixedUpdate()
    {
        applyGravity();
        performMovement();
        performRotation();

    }

    private void applyGravity()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        if (isGrounded && yVelocity.y < 0)
            yVelocity.y = -1f;

        yVelocity.y += gravity * Time.fixedDeltaTime;
        yVelocity = Vector3.ClampMagnitude(yVelocity, -gravity);
        CController.Move(yVelocity * Time.fixedDeltaTime);
    }

    public void Jump()
    {
        if (!isGrounded)
            return;

        yVelocity.y = Mathf.Sqrt(jumpHeight * -2 * gravity);
    }

    private void performRotation()
    {
        transform.Rotate(Vector3.up * rotation);
        cam.transform.localRotation = Quaternion.Euler(-camRotation, 0f, 0f);
    }

    private void performMovement()
    {
        if (velocity == Vector3.zero)
            return;
        Vector3 velocityToApply = isSprinting ? velocity * sprintMultiplier : velocity;
        CController.Move(velocityToApply * Time.fixedDeltaTime);
    }

    public void Move(Vector3 velocity)
    {
        if (!isGrounded)
            return;
        this.velocity = velocity;
    }

    public void Rotate(float rotation)
    {
        this.rotation = rotation;
    }

    public void RotateCamera(float camRotation)
    {
        this.camRotation += camRotation;
        this.camRotation = Mathf.Clamp(this.camRotation, -90f, 90f);
    }

    public void Sprinting(bool isSprinting)
    {
        if (!isGrounded)
            return;
        this.isSprinting = isSprinting;
    }
}
