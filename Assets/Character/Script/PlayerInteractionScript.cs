using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerControllerScript))]
public class PlayerInteractionScript : MonoBehaviour
{
    [SerializeField]
    private Camera cam;
    [SerializeField]
    private float interactionDistance;

    private PlayerControllerScript playerController;

    private GameObject objectInFrontOfPlayer;

    // Start is called before the first frame update
    void Start()
    {
        playerController = GetComponent<PlayerControllerScript>();
    }

    // Update is called once per frame
    void Update()
    {
        CheckWhatsInFrontOfPlayer();
        ReactToWhatsInFrontOfPlayer();
    }

    private void CheckWhatsInFrontOfPlayer()
    {
        Ray ray = cam.ScreenPointToRay(new Vector3(cam.pixelWidth * 0.5f, cam.pixelHeight * 0.5f));
        if (Physics.Raycast(ray, out RaycastHit raycastHit, interactionDistance))
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
        if(!objectInFrontOfPlayer)
        {
            playerController.playerHUDScript.SetTextUnderCrosshairVisibility(false);
            return;
        }

        bool shouldShowText = false;

        playerController.playerHUDScript.SetTextUnderCrosshairVisibility(shouldShowText);
    }

    private void InteractWithWhatsInFrontOfPlayer()
    {
        if (!objectInFrontOfPlayer)
        {
            return;
        }
    }
}
