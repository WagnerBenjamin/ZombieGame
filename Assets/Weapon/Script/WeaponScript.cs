using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class WeaponScript : MonoBehaviour
{
    int ammoLeft;
    public int AmmoLeft
    {
        get
        {
            return ammoLeft;
        }
        set
        {
            ammoLeft = value;
            if(owningPlayer)
                owningPlayer.playerHUDScript.UpdateGunAmmo(AmmoMax, ammoLeft);
        }
    }
    float nextFireTime;
    float spreadPercent;
    PlayerControllerScript owningPlayer;
    bool isAiming;
    bool Reloading;

    AudioSource audioData;    

    [Header("Gun Property")]
    [SerializeField]
    int AmmoMax;
    [SerializeField]
    float ReloadTime;
    [SerializeField]
    float FireRate;
    [SerializeField]
    int Damage;
    [SerializeField]
    float SpreadAdd;
    [SerializeField]
    float recoilAdd;
    [Header("Audio")]
    [SerializeField]
    AudioClip shootFx;
    [SerializeField]
    AudioClip reloadFx;
    [SerializeField]
    Camera SightCamera;

    // Start is called before the first frame update
    void Start()
    {
        AmmoLeft = AmmoMax;
        audioData = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if(spreadPercent > 0)
        {
            spreadPercent -= Time.deltaTime * 100;
        }
        else
        {
            spreadPercent = 0;
        }
    }

    public async void Reload()
    {
        Reloading = true;
        await Task.Delay(System.TimeSpan.FromSeconds(ReloadTime));
        AmmoLeft = AmmoMax;
        audioData.PlayOneShot(reloadFx);
        Reloading = false;
    }

    public void SetAimingMode(bool adsMode)
    {
        isAiming = adsMode;
    }

    public bool Shoot()
    {
        if(Time.time > nextFireTime && AmmoLeft > 0 && !Reloading)
        {
            //Info de base du tir
            Camera camToUse = isAiming ? SightCamera : owningPlayer.getPlayerCamera();
            Vector3 camCenter = camToUse.ScreenToWorldPoint(new Vector3(camToUse.pixelWidth * 0.5f, camToUse.pixelHeight * 0.5f));
            Vector3 shootDir = camToUse.transform.forward;

            //Si on ne vise pas, alors on donne un angle aléatoire basé sur le spreadPercent
            if(!isAiming)
            {
                float xAngle = Random.Range(-spreadPercent * 0.05f, spreadPercent * 0.05f);
                float yAngle = Random.Range(-spreadPercent * 0.05f, spreadPercent * 0.05f);
                shootDir = Quaternion.AngleAxis(xAngle, Vector3.up) * shootDir;
                shootDir = Quaternion.AngleAxis(yAngle, Vector3.forward) * shootDir;
            }

            //Mise a jour des infos
            spreadPercent += SpreadAdd;
            if (spreadPercent > 100)
            {
                spreadPercent = 100;
            }
            nextFireTime = Time.time + FireRate;

            audioData.PlayOneShot(shootFx);

            if (isAiming)
            {
                //owningPlayer.AddRecoil(recoilAdd);
            }

            AmmoLeft--;

            //Tir
            Ray ray = new Ray(camCenter, shootDir);
            if(Physics.Raycast(ray, out RaycastHit raycastHit))
            {
                ZombieScript zbScript = raycastHit.collider.gameObject.GetComponentInParent<ZombieScript>();
                if (zbScript)
                {
                    return zbScript.TakeDamage(Damage, raycastHit.collider.name);
                }
            }
        }

        return false;
    }

    public Camera GetSightCamera()
    {
        return SightCamera;
    }

    public int GetRemainingAmmo()
    {
        return AmmoLeft;
    }

    public int GetMaxAmmo()
    {
        return AmmoMax;
    }

    public float GetSpread()
    {
        return spreadPercent;
    }

    public void SetOwningPlayer(PlayerControllerScript player)
    {
        owningPlayer = player;
    }
}
