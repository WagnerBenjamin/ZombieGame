using System;
using UnityEngine;

public class PlayerScript : MonoBehaviour
{
    Vector3 moveVelocity;
    Vector3 yVelocity;
    CharacterController CController;

    float xRotation;
    float xRecoilRotation;
    bool isGrounded;
    bool isSprinting;
    float gravity = -9.8f;
    int playerHealth = 100;
    bool isDead;
    GameControllerScript gcs;

    Canvas playerHUD;
    public PlayerHUDScript playerHUDScript { get; private set; }
    Canvas pauseHUD;
    public Camera playerCamera { get; private set; }
    GameObject gunInHand;
    WeaponScript gunInHandScript;

    GameObject objectInFrontOfPlayer;

    [Header("Object")]
    [SerializeField]
    Transform groundCheck;
    [SerializeField]
    LayerMask groundMask;
    [SerializeField]
    Canvas PlayerHUDPrefab;
    [SerializeField]
    Canvas PauseHUDPrefab;
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

    private float recoilSpeed = 0.1f;
    private float RecoilTime;
    private float aimSpeed = 6;
    private bool isAiming = false;
    private float lerpGun;
    private float LerpGun
    {
        get { return lerpGun; }
        set
        {
            if (value > 1)
            {
                lerpGun = 1;
                isAiming = true;
                if(gunInHandScript)
                {
                    playerCamera.enabled = false;
                    gunInHandScript.GetSightCamera().enabled = true;
                }
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
                if (gunInHandScript)
                {
                    gunInHandScript.GetSightCamera().enabled = false;
                    playerCamera.enabled = true;
                }
            }

            playerHUDScript?.ShowCrosshair(!isAiming);
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
        playerHUD = Instantiate(PlayerHUDPrefab);
        playerHUDScript = playerHUD.GetComponent<PlayerHUDScript>();
        pauseHUD = Instantiate(PauseHUDPrefab);
        pauseHUD.enabled = false;
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

        float tt = xRecoilRotation * recoilSpeed;
        xRecoilRotation -= tt;
        xRotation -= mouseY + tt;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);
        playerCamera.transform.localRotation = Quaternion.Euler(xRotation, 0, 0f);

        transform.Rotate(Vector3.up * mouseX);
    }

    private void SetCrosshair()
    {
        if (gunInHand)
        {
            playerHUDScript.SetCrosshairSize(gunInHandScript.GetSpread());

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
            playerHUDScript.ShowCrosshair(false);
            enabled = false;
        }
        playerHUDScript.UpdateHealth(playerHealth, playerHealthMax);
    }

    private void ChangeWeapon(GameObject newWeapon)
    {
        Destroy(gunInHand);
        if(newWeapon)
        {
            gunInHand = Instantiate(newWeapon, gunHold.transform);
            gunInHandScript = gunInHand.GetComponent<WeaponScript>();
            gunInHandScript.SetOwningPlayer(this);
            playerHUDScript.UpdateGunAmmo(true, gunInHandScript.GetMaxAmmo(), gunInHandScript.GetRemainingAmmo());
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
                playerHUDScript.SetTextUnderCrosshair("Appuyez sur \"F\" pour acheter");
                playerHUDScript.SetTextUnderCrosshairVisibility(true);
            }
        }
        else
        {
            playerHUDScript.SetTextUnderCrosshairVisibility(false);
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
                    playerHUDScript.ShowHitIndicator();
                }
            }
        }

        if(Input.GetButtonDown("Menu"))
        {
            gcs.PauseControl();
        }

        if(Input.GetButtonDown("Reload"))
        {
            gunInHandScript.Reload();
        }

        LerpGun = Input.GetButton("Aiming") ? LerpGun + Time.deltaTime * aimSpeed : LerpGun - Time.deltaTime * aimSpeed;
        if (gunHold)
        {
            gunHold.transform.position = Vector3.Lerp(shootingPos[0].position, shootingPos[1].position, LerpGun);
        }
    }

    public void ShowPauseMenu()
    {
        playerHUD.enabled = !gcs.gamePaused;
        pauseHUD.enabled = gcs.gamePaused;
        Cursor.visible = gcs.gamePaused;
        Cursor.lockState = gcs.gamePaused ? CursorLockMode.Confined : CursorLockMode.Locked;
    }

    public void AddRecoil(float recoilToAdd)
    {
        xRecoilRotation += recoilToAdd;
        //xRotation -= recoilToAdd;
    }
}
