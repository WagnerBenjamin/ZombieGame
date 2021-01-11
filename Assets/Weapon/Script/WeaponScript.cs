using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class WeaponScript : MonoBehaviour
{
    int ammoLeft;
    float nextFireTime;
    float spreadPercent;
    Camera playerCam;
    bool isAiming = false;

    AudioSource audioData;    

    [SerializeField]
    int AmmoMax;
    [SerializeField]
    float ReloadTime;
    [SerializeField]
    float FireRate;
    [SerializeField]
    int Damage;
    [SerializeField]
    Transform ShootingPoint;
    [SerializeField]
    float SpreadAdd;
    [Header("Audio")]
    [SerializeField]
    AudioClip shootFx;
    [SerializeField]
    AudioClip reloadFx;

    // Start is called before the first frame update
    void Start()
    {
        ammoLeft = AmmoMax;
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

    public Transform GetShootingPoint()
    {
        return ShootingPoint;
    }

    public void Reload()
    {
        throw new System.NotImplementedException();
    }

    public void SetAimingMode(bool adsMode)
    {
        isAiming = adsMode;
    }

    public bool Shoot()
    {
        if(Time.time > nextFireTime)
        {
            //Info de base du tir
            Vector3 camCenter = playerCam.ScreenToWorldPoint(new Vector3(playerCam.pixelWidth * 0.5f, playerCam.pixelHeight * 0.5f));
            Vector3 shootDir = playerCam.transform.forward;

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

    public int GetRemainingAmmo()
    {
        throw new System.NotImplementedException();
    }

    public int GetMaxAmmo()
    {
        throw new System.NotImplementedException();
    }

    public float GetSpread()
    {
        return spreadPercent;
    }

    public void SetCamera(Camera camera)
    {
        playerCam = camera;
    }
}
