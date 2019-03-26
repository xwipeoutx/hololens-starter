using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArtilleryFire : MonoBehaviour
{
    public GameObject bullet;
    public Transform turret;
    public AudioSource audioSource;
    public ParticleSystem particleSystem;

    public float bulletSpeed = 0.5f;
    public float timeBetweenShots = 1;
    public float bulletLifetime = 5;

    private float nextShotTime;

    void Start()
    {
        nextShotTime = Time.time + timeBetweenShots;
    }
    
    // Update is called once per frame
    void Update()
    {
        if (Time.time > nextShotTime)
        {
            StartCoroutine(FireBullet());
            nextShotTime = Time.time + timeBetweenShots;
        }
    }

    private IEnumerator FireBullet()
    {
        var newBullet = Instantiate(bullet, transform);
        audioSource.Play();
        particleSystem.Play();

        var rigidBody = newBullet.GetComponent<Rigidbody>();
        rigidBody.velocity = turret.forward * bulletSpeed;
        

        yield return new WaitForSeconds(bulletLifetime);
        Destroy(newBullet);

    }
}
