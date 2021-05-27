using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class weaponSCR : MonoBehaviour
{
    private GameObject player;

    public int id;
    public string name;
    public float damage;
    public float fireRate;
    public float maxDistance;
    public float reloadTime;
    public int bulletSize;
    public int bulletsInGun;
    public int bulletMaxInGun;
    
    //shooting
    private float nextFire;
    public GameObject barrel;//gunEnd
    private Camera fpsCam;
    private WaitForSeconds shotDuration = new WaitForSeconds(0.07f);
    private WaitForSeconds reloadingDuration;
    private LineRenderer laserLine;
    private AudioSource gunAudio;
    public AudioSource reloadingSound;
    public float tempReload;
    animationSCR p;
    private bool temp2 = true;
    bool reloading;
    public float reloadingTimeLive;
    //sound eff
    bool onceT0 = true;
    void Start()
    {
        laserLine = GetComponent<LineRenderer>();
        fpsCam = GameObject.FindGameObjectWithTag("MainCamera").GetComponentInParent<Camera>();
        //GameObject tempGO = gameObject.transform.parent.GetChild(0);
        //fpsCam = gameObject.transform.parent.GetComponentInChildren<Camera>();
        gunAudio = GetComponent<AudioSource>();
        player = gameObject.transform.parent.gameObject;
        p = player.GetComponent<animationSCR>();
        reloadingDuration = new WaitForSeconds(reloadTime);
    }
    public void shoot()
    {
        bool hasBullets = false;
        
        if(bulletSize == 9)
        {
            if(p.bullets9mm > 0)
            {
                hasBullets = true;
            }
        }else if(bulletSize == 12)
        {
            if(p.bullets12mm > 0)
            {
                hasBullets = true;
            }
        }
        bool reloaded = false;
        if(bulletsInGun > 0)
        {
            reloaded = true;
        }
        else
        {
            if (temp2)
            {
                temp2 = false;
                tempReload = Time.time + reloadTime;
                reloading = true;
            }
            if (Time.time > tempReload)
            {
                temp2 = true;
                reloaded = true;
                reloading = false;
                bulletsInGun = bulletMaxInGun;
            }
        }
        if(Time.time > nextFire && hasBullets && reloaded)
        {
            bulletsInGun--;
            if (bulletSize == 9)
            {
                p.bullets9mm--;
            }
            else if(bulletSize == 12)
            {
                p.bullets12mm--;
            }
            nextFire = Time.time + fireRate;
            StartCoroutine(ShotEffect());
            Vector3 rayOrigin = fpsCam.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, 0.0f));
            RaycastHit hit;
            laserLine.SetPosition(0, barrel.transform.position);
            if (Physics.Raycast(rayOrigin, fpsCam.transform.forward, out hit, maxDistance))
            {
                laserLine.SetPosition(1, hit.point);
                enemySCR health = hit.collider.GetComponent<enemySCR>();
                Debug.DrawRay(rayOrigin, fpsCam.transform.forward * maxDistance, Color.green);
                if (health != null)
                {
                    health.Damage(damage);
                }

                // Check if the object we hit has a rigidbody attached
                if (hit.rigidbody != null)
                {
                    hit.rigidbody.AddForce(-hit.normal * 10);
                }
                if (hit.collider.GetComponent<criticalDamage>() != null)
                {
                    hit.collider.gameObject.GetComponent<criticalDamage>().kill();
                }
            }
            else
            { 
                laserLine.SetPosition(1, rayOrigin + (fpsCam.transform.forward * maxDistance));
            }
        }
    }
    private void FixedUpdate()
    {
        if (reloading)
        {
            if (onceT0)
            {
                onceT0 = false;
                StartCoroutine(reloadingEffect());
            }
            reloadingTimeLive = (tempReload - Time.time);
            Debug.Log("time left to reload : " + (reloadingTimeLive));
            if(reloadingTimeLive <= 0)
            {
                reloading = false;
            }
        }
    }
    private IEnumerator ShotEffect()
    {
        // Play the shooting sound effect
        gunAudio.Play();
        player.GetComponent<animationSCR>().weaponSound = true;
        // Turn on our line renderer
        laserLine.enabled = true;

        //Wait for .07 seconds
        yield return shotDuration;
        player.GetComponent<animationSCR>().weaponSound = false;
        // Deactivate our line renderer after waiting
        laserLine.enabled = false;
    }
    private IEnumerator reloadingEffect()
    {
        reloadingSound.Play();
        yield return reloadingDuration;
        onceT0 = true;
    }
}
