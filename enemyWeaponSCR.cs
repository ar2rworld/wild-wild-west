using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class enemyWeaponSCR : MonoBehaviour
{
    public int id;
    public string name;
    public float damage;
    public float fireRate;
    public float maxDistance;
    public float reloadTime;

    //shooting
    private float nextFire;
    public GameObject barrel;//gunEnd
    private WaitForSeconds shotDuration;
    private LineRenderer laserLine;
    private AudioSource gunAudio;
    void Start()
    {
        laserLine = GetComponent<LineRenderer>();
        gunAudio = GetComponent<AudioSource>();
        shotDuration = new WaitForSeconds(fireRate);
    }
    private void Update()
    {
        Vector3 rayOrigin = barrel.transform.position;
        Debug.DrawRay(rayOrigin, barrel.transform.forward * maxDistance, Color.green);
    }

    public void shoot()
    {
        if (Time.time > nextFire)
        {
            nextFire = Time.time + fireRate;
            StartCoroutine(ShotEffect());
            Vector3 rayOrigin = barrel.transform.position;
            RaycastHit hit;
            laserLine.SetPosition(0, barrel.transform.position);
            if (Physics.Raycast(rayOrigin, barrel.transform.forward, out hit, maxDistance))
            {
                laserLine.SetPosition(1, hit.point);
                animationSCR health = hit.collider.GetComponent<animationSCR>();
                if (health != null)
                {
                    health.Damage(damage);
                }

                // Check if the object we hit has a rigidbody attached
                if (hit.rigidbody != null)
                {
                    hit.rigidbody.AddForce(-hit.normal * 10);
                }
            }
            else
            {
                laserLine.SetPosition(1, rayOrigin + (barrel.transform.forward * maxDistance));
            }
        }
    }
    private IEnumerator ShotEffect()
    {
        // Play the shooting sound effect
        gunAudio.Play();

        // Turn on our line renderer
        laserLine.enabled = true;

        //Wait for .07 seconds
        yield return shotDuration;

        // Deactivate our line renderer after waiting
        laserLine.enabled = false;
    }
}
