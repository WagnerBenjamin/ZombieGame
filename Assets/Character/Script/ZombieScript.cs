using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(AudioSource))]
public class ZombieScript : MonoBehaviour
{
    GameObject player;
    NavMeshAgent nav;
    float timeBeforeDestroy;
    float timeBeforeCanHit;
    float timeBeforeGrowl;
    AudioSource audioSource;
    Animator animator;

    bool isDead;
    public bool IsDead
    {
        get { return isDead; }
    }

    [SerializeField]
    int damage;
    [SerializeField]
    int zombieHealth;
    [SerializeField]
    int timeUntilDestroy;
    [SerializeField]
    AudioClip[] ZombieSounds;

    // Start is called before the first frame update
    void Start()
    {
        nav = GetComponent<NavMeshAgent>();
        player = GameObject.FindGameObjectWithTag("Player");
        audioSource = GetComponent<AudioSource>();
        animator = GetComponentInChildren<Animator>();
        timeBeforeGrowl = Time.time + Random.Range(5, 15);
    }

    // Update is called once per frame
    void Update()
    {
        if(Time.time > timeBeforeGrowl && !isDead)
        {
            int rand = Random.Range(0, ZombieSounds.Length - 1);
            audioSource.PlayOneShot(ZombieSounds[rand]);

            timeBeforeGrowl = Time.time + Random.Range(5, 30);
        }

        if(isDead)
        {
            animator.SetBool("isDead", true);
            if (Time.time > timeBeforeDestroy)
            {
                Destroy(gameObject);
            }
        }
        else
        {
            nav.SetDestination(player.transform.position);
            if (Vector3.Distance(transform.position, player.transform.position) < 1.5f)
            {
                if(Time.time > timeBeforeCanHit)
                {
                    player.GetComponent<PlayerScript>().TakeDamage(damage);
                    animator.SetBool("attack", true);
                    timeBeforeCanHit = Time.time + 1;
                }
            }
            else
            {
                animator.SetBool("attack", false);
            }
        }

        animator.SetFloat("speed", nav.velocity.magnitude);
    }

    public bool TakeDamage(int damage, string bodyPart)
    {
        if (isDead)
            return false;
        Debug.Log(bodyPart);
        zombieHealth -= bodyPart == "Head" ? damage * 2 : damage;
        if(zombieHealth <= 0)
        {
            zombieHealth = 0;
            nav.isStopped = true;
            nav.enabled = false;
            //transform.rotation = Quaternion.Euler(0, 0, -90);
            foreach(var coll in GetComponentsInChildren<Collider>())
            {
                coll.enabled = false;
            }
            timeBeforeDestroy = Time.time + 20;
            isDead = true;

            GameControllerScript gcs = (GameControllerScript)FindObjectOfType(typeof(GameControllerScript));
            gcs.removeDeadZombieFromList(gameObject);
        }

        return true;
    }
}
