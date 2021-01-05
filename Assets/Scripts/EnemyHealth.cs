using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyHealth : MonoBehaviour
{
    public GameObject enemyShip;
    public Slider slider;
    public int score;

    // Start is called before the first frame update
    void Start()
    {
        transform.position = new Vector3(enemyShip.transform.position.x, enemyShip.transform.position.y + 1, enemyShip.transform.position.z);
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = new Vector3(enemyShip.transform.position.x, enemyShip.transform.position.y + 1, enemyShip.transform.position.z);
    }

    public void SetMaxHealth(float health)
    {
        slider.maxValue = health;
        slider.value = health;
    }

    public void SetHealth(float health)
    {
        slider.value = health;
        if(health <= 0){
            score = PlayerPrefs.GetInt("score") + 10;
            Debug.Log(score);
            PlayerPrefs.SetInt("score", score);
            Destroy(gameObject);
        }
    }


}
