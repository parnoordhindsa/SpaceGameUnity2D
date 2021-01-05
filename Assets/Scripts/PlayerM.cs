using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.IO;


public class PlayerM : MonoBehaviour, TakesDamage
{
    // level 0 speed upgrade gives the player this value
    public const float BASE_SPEED = 7.0f;
    // each level of speed upgrade multiplies the previous level by this much
    public const float SPEED_MULTIPLIER = BASE_SPEED * 0.1f;

    public const float BASE_ACCELERATION = BASE_SPEED * 2.0f;
    public const float ACCELERATION_MULTIPLIER = BASE_ACCELERATION * 0.1f;

    public const float BASE_HEALTH = 20.0f;
    public const float HEALTH_MULTIPLIER = BASE_HEALTH * 0.1f;

    // max speed of the player
    private float moveSpeed;
    // how fast the player slows doen when not moving (per second)
    private float decelerationRate = BASE_SPEED / 2.0f;
    // how fast the player speeds up (per second)
    private float accelerationRate;

    public Rigidbody2D rb;

    public float currentHealth { get; set; }
    public float maxHealth { get; set; }

    public GameObject backgroundMusic1;
    public GameObject backgroundMusic2;
    public GameObject backgroundMusic3;
    private GameObject backgroundMusic;

    public Slider HealthBarPlayer;

    public ProjectileBehaviour ProjectilePrefab;
    public Transform LaunchOffset;

    private Animator anim;
    private Vector3 change;

    public string theFilename;
    public PlayerData playerData;


    void Start()
    {
        anim = GetComponent<Animator>();
        //HealthBarPlayer.value = CalculateHealth();
        SetBackgroundMusic();
    }

    // player health bar must be assigned
    // change stats i.e. health, speed, acceleration
    public void Initialize(Slider slider, PlayerUpgrades playerUpgrades)
    {
        HealthBarPlayer = slider;
        maxHealth = BASE_HEALTH + playerUpgrades.healthLevel * HEALTH_MULTIPLIER;
        currentHealth = maxHealth;
        moveSpeed = BASE_SPEED + playerUpgrades.speedLevel * SPEED_MULTIPLIER;
        accelerationRate = BASE_ACCELERATION + playerUpgrades.accelerationLevel * ACCELERATION_MULTIPLIER;
    }

    public void FixedUpdate()
    {
        // get current mouse position on screen
        Vector2 mousePos = (Vector2)Input.mousePosition;
        // get current player position on screen
        Vector2 objectPos = (Vector2)Camera.main.WorldToScreenPoint(transform.position);

        mousePos.x = mousePos.x - objectPos.x;
        mousePos.y = mousePos.y - objectPos.y;

        // update player rotation
        float angle = Mathf.Atan2(mousePos.y, mousePos.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(Vector3.forward * (angle + 270.0f));

        // move player
        if (Input.GetKey(KeyCode.Space))
        {
            // get facing vector
            Vector2 facing = new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad));

            rb.AddForce(facing * accelerationRate);
        }

        // decelerate player
        float speed = rb.velocity.magnitude;
        if (speed > moveSpeed)
        {
            rb.velocity = rb.velocity * moveSpeed / speed;
        }
        else if (speed != 0.0f)
        {
            // set velocity to decreased speed value
            rb.velocity = rb.velocity * (speed - decelerationRate * Time.fixedDeltaTime) / speed;
        }
    }

    public void TakeDamage(float damageValue)
    {
        currentHealth -= damageValue;
        HealthBarPlayer.value = CalculateHealth();
        if (currentHealth <= 0)
        {
            // reset player statistics when player dies
            theFilename = PlayerPrefs.GetString("filename");
            playerData.filename = theFilename;
            SavePlayerDataJson();

            Destroy(gameObject);
            SceneManager.LoadScene("GameoverScene");
            Debug.Log("You Dead");
        }
    }

    public void OnCollisionEnter2D(Collision2D collision)
    {
        TakesDamage collider = collision.collider.gameObject.GetComponent<TakesDamage>();
        collider?.TakeDamage(2.0f);
    }

    float CalculateHealth()
    {
        return currentHealth / maxHealth;
    }

    void ProcessInput()
    {
        //float moveX = Input.GetAxisRaw("Horizontal");
        //float moveY = Input.GetAxisRaw("Vertical");

        //moveDirection = new Vector2(moveX,moveY).normalized;
    }

    private void SetBackgroundMusic()
    {
        switch(Random.Range(0, 3))
        {
            case 0:
                GameObject backgroundMusicObject1 = Instantiate(backgroundMusic1);
                break;
            case 1:
                GameObject backgroundMusicObject2 = Instantiate(backgroundMusic2);
                break;
            case 2:
                GameObject backgroundMusicObject3 = Instantiate(backgroundMusic3);
                break;
        }
    }

    [ContextMenu("To Json Data")]
    void SavePlayerDataJson()
    {
        string jsonData = JsonUtility.ToJson(playerData, true);
        Debug.Log(jsonData);
        string path = Application.persistentDataPath + "/" + theFilename + ".json";
        File.WriteAllText(path, jsonData);
    }

}
