using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShootScript : MonoBehaviour
{
    [SerializeField]
    private Transform gunHold;

    GameObject gunInHand;
    WeaponScript gunInHandScript;

    private PlayerControllerScript playerController;

    // Start is called before the first frame update
    void Start()
    {
        playerController = GetComponent<PlayerControllerScript>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void setWeapon(GameObject newWeapon)
    {
        Destroy(gunInHand);
        if (newWeapon)
        {
            gunInHand = Instantiate(newWeapon, gunHold.transform);
            gunInHandScript = gunInHand.GetComponent<WeaponScript>();
            gunInHandScript.SetOwningPlayer(playerController);
            playerController.playerHUDScript.UpdateGunAmmo(true, gunInHandScript.GetMaxAmmo(), gunInHandScript.GetRemainingAmmo());
        }
    }
}
