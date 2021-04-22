using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHUDScript : MonoBehaviour
{
    float hitIndicatorHideTime;
    bool hitIndicatorDisplayed;
    GameControllerScript gcs;

    [SerializeField]
    private Canvas PlayerHUDPrefab;

    [SerializeField]
    RectTransform crosshair;
    [SerializeField]
    Text textUnderCrosshair;
    [SerializeField]
    GameObject hitIndicator;
    [SerializeField]
    Text ZombieCount;
    [SerializeField]
    Text PlayerHealth;
    [SerializeField]
    Text round;
    [SerializeField]
    Text point;
    [SerializeField]
    Image hitImage;
    [SerializeField]
    Text GunAmmo;

    private Canvas playerHUD;

    // Start is called before the first frame update
    void Start()
    {
        playerHUD = Instantiate(PlayerHUDPrefab);
        gcs = (GameControllerScript)FindObjectOfType(typeof(GameControllerScript));
        textUnderCrosshair.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        if(hitIndicatorDisplayed && Time.time > hitIndicatorHideTime)
        {
            hitIndicatorDisplayed = false;
            hitIndicator.SetActive(false);
        }

        UpdateGCSVariable();
    }

    private void UpdateGCSVariable()
    {
        ZombieCount.text = gcs.SpawnedZombie.Count + " / " + gcs.MaxZombie;
        round.text = gcs.getRound().ToString();
        point.text = gcs.playerPoint.ToString();
    }

    public void SetCrosshair(Vector3 position = new Vector3())
    {
        crosshair.transform.position = position;
    }

    public void SetCrosshairSize(float sizePercent)
    {
        crosshair.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, sizePercent + 45);
        crosshair.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, sizePercent + 45);
    }

    public void ShowCrosshair(bool visibility)
    {
        crosshair.localScale = visibility ? new Vector3(1, 1) : new Vector3(0, 0);
    }

    public void SetTextUnderCrosshair(string text)
    {
        textUnderCrosshair.text = text;
    }

    public void SetTextUnderCrosshairVisibility(bool isVisible)
    {
        textUnderCrosshair.enabled = isVisible;
    }

    public void ShowHitIndicator()
    {
        hitIndicator.SetActive(true);
        hitIndicatorHideTime = Time.time + 0.1f;
        hitIndicatorDisplayed = true;
    }

    public void UpdateHealth(int currHealth, int maxHealth)
    {
        PlayerHealth.text = currHealth + " / " + maxHealth;
        hitImage.canvasRenderer.SetAlpha(.3f);
        hitImage.color = Color.white;
        hitImage.CrossFadeAlpha(0.0f, .5f, false);
    }

    public void UpdateGunAmmo(int maxAmmo, int remainingAmmo)
    {
        GunAmmo.text = remainingAmmo + " / " + maxAmmo;
    }

    public void UpdateGunAmmo(bool hasGunInHand, int maxAmmo, int remainingAmmo)
    {
        GunAmmo.enabled = hasGunInHand;
        UpdateGunAmmo(maxAmmo, remainingAmmo);
    }
}
