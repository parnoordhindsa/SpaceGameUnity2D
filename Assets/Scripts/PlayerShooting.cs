using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class PlayerShooting : MonoBehaviour
{

    public Transform firePointRight;
    public Transform firePointLeft;
    public GameObject playerFireRight;
    public Animator anim;

    // This will control how fast the projectile travels
    public float bulletLifetime;
    public float bulletSpeed;

    private PlayerUpgrades playerUpgrades;

    private Rigidbody2D rb;

    void Start() {
        bulletLifetime = 1.0f;
        bulletSpeed = 20f;
        rb = GetComponent<Rigidbody2D>();
    }

    public void Initialize(PlayerUpgrades upgrades)
    {
        playerUpgrades = upgrades;
    }

    // Update is called once per frame
    void Update()
    {
        if (anim.GetFloat("moveY") > 0f) // If moving up
        {
            firePointRight.localPosition = new Vector3(0.082f, 0, 0);
            firePointLeft.localPosition = new Vector3(-0.082f, 0, 0);
            if (true)
            {
                firePointRight.transform.rotation = Quaternion.Euler(0, 0, 360f);
                firePointLeft.transform.rotation = Quaternion.Euler(0, 0, 360f);
            }
        }
        else if (anim.GetFloat("moveY") < 0f) // If moving down
        {
            firePointRight.localPosition = new Vector3(-0.082f, 0, 0);
            firePointLeft.localPosition = new Vector3(0.082f, 0, 0);
            if (true)
            {
                firePointRight.transform.rotation = Quaternion.Euler(0, 0, 180f);
                firePointLeft.transform.rotation = Quaternion.Euler(0, 0, 180f);
            }
        }
        else if (anim.GetFloat("moveX") > 0f) // If moving right
        {
            firePointRight.localPosition = new Vector3(0, -0.082f, 0);
            firePointLeft.localPosition = new Vector3(0, 0.082f, 0);
            if (true) // Only changes the fire angle once
            {
                firePointRight.transform.rotation = Quaternion.Euler(0, 0, -90f);
                firePointLeft.transform.rotation = Quaternion.Euler(0, 0, -90f);
            }
        }
        else if (anim.GetFloat("moveX") < 0f) // If moving left
        {
            firePointRight.localPosition = new Vector3(0, 0.082f, 0);
            firePointLeft.localPosition = new Vector3(0, -0.082f, 0);
            if (true)
            {
                firePointRight.transform.rotation = Quaternion.Euler(0, 0, -270f);
                firePointLeft.transform.rotation = Quaternion.Euler(0, 0, -270f);
            }
        }

        if (Input.GetMouseButtonDown(0) && Time.timeScale > 0.0f)
        { // don't shooot if game is paused
            Shoot();
        }
    }

    void Shoot()
    {
        // Creates the attack, shoots in the direction of the firepoint, destroys after amount of time
        GameObject laserRight = Instantiate(playerFireRight, firePointRight.position, firePointRight.rotation);
        laserRight.GetComponent<PlayerFireRight>().Initialize(playerUpgrades);
        GameObject laserLeft = Instantiate(playerFireRight, firePointLeft.position, firePointLeft.rotation);
        laserLeft.GetComponent<PlayerFireRight>().Initialize(playerUpgrades);
        Rigidbody2D rbRight = laserRight.GetComponent<Rigidbody2D>();
        Rigidbody2D rbLeft = laserLeft.GetComponent<Rigidbody2D>();
        float dot = Vector2.Dot(rb.velocity.normalized, firePointLeft.up);
        if (dot < 0.0f)
            dot = 0.0f;
        rbRight.AddForce(rb.velocity * dot + (Vector2)firePointRight.up * bulletSpeed, ForceMode2D.Impulse);
        rbLeft.AddForce(rb.velocity * dot + (Vector2)firePointLeft.up * bulletSpeed, ForceMode2D.Impulse);
        Destroy(laserRight, bulletLifetime);
        Destroy(laserLeft, bulletLifetime);
    }
}
