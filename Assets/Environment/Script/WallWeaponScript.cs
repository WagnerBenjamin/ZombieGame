using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallWeaponScript : MonoBehaviour
{
    [SerializeField]
    GameObject WeaponOnTheWall;
    [SerializeField]
    Transform WeaponHolder;

    // Start is called before the first frame update
    void Start()
    {
        Instantiate(WeaponOnTheWall, WeaponHolder);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public GameObject GetWeapon()
    {
        return WeaponOnTheWall;
    }
}
