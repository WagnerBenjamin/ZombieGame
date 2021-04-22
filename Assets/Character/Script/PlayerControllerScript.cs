using Mirror;
using UnityEngine;

[RequireComponent(typeof(PlayerMovementScript))]
[RequireComponent(typeof(PlayerShootScript))]
public class PlayerControllerScript : NetworkBehaviour
{
    [SerializeField]
    private float speed;
    [SerializeField]
    private float mouseSensitivityX;
    [SerializeField]
    private float mouseSensitivityY;
    [SerializeField]
    private Camera cam;

    [Header("HUD")]
    [SerializeField]
    private Canvas PlayerHUDPrefab;
    [SerializeField]
    private Canvas PauseHUDPrefab;

    public PlayerHUDScript playerHUDScript { get; private set; }
    private Canvas playerHUD;
    private Canvas pauseHUD;

    private PlayerMovementScript pMovement;

    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;

        playerHUD = Instantiate(PlayerHUDPrefab);
        pauseHUD = Instantiate(PauseHUDPrefab);
        pauseHUD.enabled = false;

        pMovement = GetComponent<PlayerMovementScript>();
        playerHUDScript = PlayerHUDPrefab.GetComponent<PlayerHUDScript>();
    }

    // Update is called once per frame
    void Update()
    {
        UpdatePlayerMovementScript();
    }

    private void UpdatePlayerMovementScript()
    {
        float xMov = Input.GetAxisRaw("Horizontal");
        float zMov = Input.GetAxisRaw("Vertical");

        Vector3 moveHorizontal = transform.right * xMov;
        Vector3 moveVertical = transform.forward * zMov;

        Vector3 velocity = (moveHorizontal + moveVertical).normalized * speed;

        pMovement.Move(velocity);

        float yRot = Input.GetAxisRaw("Mouse X") * mouseSensitivityX;
        float xRot = Input.GetAxisRaw("Mouse Y") * mouseSensitivityY;

        pMovement.Rotate(yRot);
        pMovement.RotateCamera(xRot);

        pMovement.Sprinting(Input.GetButton("Sprint"));

        if (Input.GetButtonDown("Jump"))
        {
            pMovement.Jump();
        }
    }

    public Camera getPlayerCamera()
    {
        return cam;
    }
}
