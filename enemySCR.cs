using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine.AI;
using UnityEngine;

public class enemySCR : MonoBehaviour
{
    private GameObject player;
    public float activeDistance = 10;
    private LineRenderer laserLine;
    private Animation a;
    private int weaponID;
    public bool foundTarget = false;
    private GameObject fpsCam;
    private float randomDelay;
    private NavMeshAgent _agent;

    public float distanceToTarget;
    public float runIndex = 1.5f;
    public GameObject weapon0;
    public GameObject weapon1;
    private GameObject activeWeapon;
    public float hearingDistance = 10;
    public float health = 100;
    public float armor = 100;
    public int animStatus;
    public GameObject GameManager;
    
    bool idle = true;
    bool dead = false;
    float nextTime;
    bool aboutToDie = false;
    bool temp0 = true; //var to send playerIsFound var in GM once
    bool temp1 = true; // to randomize shooting
    private void Awake()
    {
        if(Random.value > 0.5)
        {
            weaponID = 0;
            activeWeapon = weapon0;
        }
        else
        {
            weaponID = 1;
            activeWeapon = weapon1;
        }
        activeWeapon.SetActive(false);
    }
    void Start()
    {
        _agent = GetComponent<NavMeshAgent>();
        player = GameObject.FindGameObjectWithTag("Player");
        a = GetComponent<Animation>();
        //Camera fpsCam = gameObject.transform.parent.GetComponentInChildren<Camera>();
        GameManager = GameObject.FindGameObjectWithTag("GameManager");
    }
    void FixedUpdate()
    {
        if (GameManager.GetComponent<GameManager>().playerIsFound)
        {
            foundTarget = true;
        }
        if ((getDistanceToPlayer() < 14 && isPlayerVisible() || foundTarget) && !dead)
        {
            a.Play("chasingEnemy");
            animStatus = 1;
            foundTarget = true;
            idle = false;
            activeWeapon.SetActive(true);
            if (temp0)
            {
                temp0 = false;
                GameManager.GetComponent<GameManager>().playerIsFound = true;
            }
        }
        if (foundTarget && !dead)
        {
            if (killTarget()) {
                idle = true;
            }
        }
        shootingDetection();
        if (idle && !dead)
        {
            activeWeapon.SetActive(false);
            a.Play("idleEnemy");
            animStatus = 0;
        }
        if (dead)
        {
            activeWeapon.SetActive(false);
            if (!aboutToDie)
            {
                nextTime = Time.time + 1.967f;
                aboutToDie = true;
            }
            if (animStatus == 0)
            {
                a.Play("idleToDie");
            }
            else
            {
                a.Play("chasingToDie");
            }
            if (Time.time > nextTime)
            {
                gameObject.SetActive(false);
            }
        }
    }
    bool killTarget()
    {
        bool done_ = false;
        if (player == null || player.activeSelf == false)
        {
            done_ = true;
            foundTarget = false;
        }
        else
        {
            transform.LookAt(player.transform);
            Vector3 forward_ = transform.forward;
            //transform.position += forward_ * runIndex;
            _agent.SetDestination(player.transform.position);
            if(temp1)
            {
                temp1 = false;
                randomDelay = Random.value;
            }
            else
            {
                randomDelay = 0;
            }
            float delay0 = Time.time + randomDelay;
            if (Time.time >= delay0) {
                activeWeapon.GetComponent<enemyWeaponSCR>().shoot();
            }
        }
        return done_;
    }
    float getDistanceToPlayer()
    {
        return (player.transform.position - transform.position).magnitude;
    }
    bool isPlayerVisible()
    {
        bool out_ = false;
        Vector3 ray = -transform.position + player.transform.position;// + new Vector3(0,0,0.5f);
        Debug.DrawRay(transform.position, ray, Color.green);
        RaycastHit hit;
        if (Physics.Raycast(transform.position, ray, out hit, activeDistance))
        {
            if (hit.collider.gameObject.tag == "Player" && Vector3.Angle(transform.forward, player.transform.position - transform.position) < 65)
            {//if hit to player and angle between enemy and hero is less than 65
                out_ = true;
            }
        }
        return out_;
    }
    void shootingDetection()
    {
        //Debug.Log(Vector3.Distance(transform.position, player.transform.position));
        if(Vector3.Distance(transform.position, player.transform.position) < hearingDistance && player.GetComponent<animationSCR>().weaponSound)
        {
            foundTarget = true;
        }
    }
    public void Damage(float d)
    {
        foundTarget = true;
        GameManager.GetComponent<GameManager>().playerIsFound = true;
        health -= d - d * armor / 100;
        if (health <= 0)
        {
            GameManager.GetComponent<GameManager>().incScore(1);
            player.GetComponent<animationSCR>().health += 50;
            player.GetComponent<animationSCR>().bullets12mm += 7;
            player.GetComponent<animationSCR>().bullets9mm += 10;
            dead = true;
            idle = false;
        }
    }
    private IEnumerator waitForSec(float n)
    {
        yield return new WaitForSeconds(n);
    }
    public void headShot()
    {
        health = -1;
        Damage(0);
    }
}
