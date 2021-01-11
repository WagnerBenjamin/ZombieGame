using System;
using UnityEngine;

public class PlayerScript : MonoBehaviour
{
    Vector3 moveVelocity;
    Vector3 yVelocity;
    CharacterController CController;

    float xRotation;
    bool isGrounded;
    bool isSprinting;
    float gravity = -9.8f;
    int playerHealth = 100;
    bool isDead;
    GameControllerScript gcs;
    bool gamePaused;

    PlayerHUDScript playerHUD;
    Camera playerCamera;
    GameObject gunInHand;
    WeaponScript gunInHandScript;

    GameObject objectInFrontOfPlayer;

    [Header("Object")]
    [SerializeField]
    Transform groundCheck;
    [SerializeField]
    LayerMask groundMask;
    [SerializeField]
    Canvas HUDPrefab;
    [SerializeField]
    GameObject gunHold;
    [SerializeField]
    Transform[] shootingPos;
    [Header("Move Property")]
    [SerializeField]
    float groundDistance;
    [SerializeField]
    float jumpHeight;
    [SerializeField]
    float sprintMultiplier;
    [SerializeField]
    float moveSpeed;
    [SerializeField]
    float mouseSensitivity;
    [Header("Player Property")]
    [SerializeField]
    int playerHealthMax = 100;
    [SerializeField]
    GameObject baseWeapon;
    [SerializeField]
    float interactionDistance;


    private float lerpGun;
    private float aimSpeed = 6;
    private bool isAiming = false;
    private float LerpGun
    {
        get { return lerpGun; }
        set
        {
            if (value > 1)
            {
                lerpGun = 1;
                isAiming = true;
            }
            else if (value < 0)
            {
                lerpGun = 0;
                isAiming = false;
            }
            else
            {
                lerpGun = value;
                isAiming = false;
            }

            playerHUD?.ShowCrosshair(!isAiming);
            gunInHandScript?.SetAimingMode(isAiming);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        gcs = (GameControllerScript)FindObjectOfType(typeof(GameControllerScript));
        Cursor.lockState = CursorLockMode.Locked;
        CController = GetComponent<CharacterController>();
        playerHealth = playerHealthMax;
        playerHUD = Instantiate(HUDPrefab).GetComponent<PlayerHUDScript>();
        playerCamera = GetComponentInChildren<Camera>();
        ChangeWeapon(baseWeapon);
    }

    // Update is called once per frame
    void Update()
    {
        GetMovement();
        SetLook();
        SetCrosshair();
        CheckGravity();
        ApplyMovement();

        CheckWhatsInFrontOfPlayer();
        ReactToWhatsInFrontOfPlayer();
        ReactToPlayerInput();
    }

    void GetMovement()
    {
        if(isGrounded)
        {
            isSprinting = Input.GetButton("Sprint");
            float xAxis = Input.GetAxis("Vertical");
            float zAxis = Input.GetAxis("Horizontal");

            moveVelocity = (transform.right * zAxis + transform.forward * xAxis) * moveSpeed;

            if (isSprinting)
            {
                moveVelocity *= sprintMultiplier;
            }

            if (Input.GetButtonDown("Jump"))
            {
                yVelocity.y = Mathf.Sqrt(jumpHeight * -2 * gravity);
            }
        }
    }

    private void SetLook()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);
        playerCamera.transform.localRotation = Quaternion.Euler(xRotation, 0, 0f);
        //gunHold.transform.localRotation = Quaternion.Euler(0f, -90f, -xRotation);

        transform.Rotate(Vector3.up * mouseX);
    }

    private void SetCrosshair()
    {
        if (gunInHand)
        {
            playerHUD.SetCrosshairSize(gunInHandScript.GetSpread());

            //Transform shootingPoint = gunInHandScript.GetShootingPoint();
            //Ray ray = new Ray(shootingPoint.position, shootingPoint.forward);
            //if (Physics.Raycast(ray, out RaycastHit raycastHit, 100))
            //{
            //    playerHUD.SetCrosshair(true, playerCamera.WorldToScreenPoint(raycastHit.point));
            //}
            //else
            //{
            //    playerHUD.SetCrosshair(true, playerCamera.WorldToScreenPoint(shootingPoint.position + shootingPoint.forward * 100));
            //}
        }
        else
        {
            //playerHUD.SetCrosshair(true);
        }
    }

    private void CheckGravity()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        if (isGrounded && yVelocity.y < 0)
        {
            yVelocity.y = -2f;
        }

        yVelocity.y += gravity * Time.deltaTime;
    }

    private void ApplyMovement()
    {
        CController.Move(moveVelocity * Time.deltaTime);
        CController.Move(yVelocity * Time.deltaTime);
    }

    public void TakeDamage(int damage)
    {
        playerHealth -= damage;
        if(playerHealth <= 0)
        {
            playerHealth = 0;
            isDead = true;
            GetComponentInChildren<Camera>().enabled = false;
            gcs.GameCam.enabled = true;
            playerHUD.ShowCrosshair(false);
            enabled = false;
        }
        playerHUD.UpdateHealth(playerHealth, playerHealthMax);
    }

    private void ChangeWeapon(GameObject newWeapon)
    {
        Destroy(gunInHand);
        if(newWeapon)
        {
            gunInHand = Instantiate(newWeapon, gunHold.transform);
            gunInHandScript = gunInHand.GetComponent<WeaponScript>();
            gunInHandScript.SetCamera(playerCamera);
        }
    }

    private void CheckWhatsInFrontOfPlayer()
    {
        Ray ray = new Ray(playerCamera.ScreenToWorldPoint(new Vector3(playerCamera.pixelWidth * 0.5f, playerCamera.pixelHeight * 0.5f)), playerCamera.transform.forward);
        if(Physics.Raycast(ray, out RaycastHit raycastHit, interactionDistance))
        {
            objectInFrontOfPlayer = raycastHit.collider.gameObject;
        }
        else
        {
            objectInFrontOfPlayer = null;
        }

    }

    private void ReactToWhatsInFrontOfPlayer()
    {
        if(objectInFrontOfPlayer)
        {
            if (objectInFrontOfPlayer.TryGetComponent<WallWeaponScript>(out WallWeaponScript wallWeapon))
            {
                playerHUD.SetTextUnderCrosshair("Appuyez sur \"F\" pour acheter");
                playerHUD.SetTextUnderCrosshairVisibility(true);
            }
        }
        else
        {
            playerHUD.SetTextUnderCrosshairVisibility(false);
        }
    }

    private void InteractWithWhatsInFrontOfPlayer()
    {
        if(objectInFrontOfPlayer)
        {
            if (objectInFrontOfPlayer.TryGetComponent<WallWeaponScript>(out WallWeaponScript wallWeapon))
            {
                ChangeWeapon(wallWeapon.GetWeapon());
            }
        }
    }

    private void ReactToPlayerInput()
    {
        if (Input.GetButtonDown("Interact"))
        {
            InteractWithWhatsInFrontOfPlayer();
        }

        if(Input.GetButton("Fire1"))
        {
            if(gunInHandScript)
            {
                if(gunInHandScript.Shoot())
                {
                    playerHUD.ShowHitIndicator();
                }
            }
        }

        if(Input.GetButtonDown("Menu"))
        {
            Time.timeScale = gamePaused ? 1 : 0;
            gamePaused = !gamePaused;
        }

        LerpGun = Input.GetButton("Aiming") ? LerpGun + Time.deltaTime * aimSpeed : LerpGun - Time.deltaTime * aimSpeed;
        if (gunHold)
            gunHold.transform.position = Vector3.Lerp(shootingPos[0].position, shootingPos[1].position, LerpGun);
    }
}
