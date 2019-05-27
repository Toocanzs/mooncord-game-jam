using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
[DefaultExecutionOrder(-500000)]
public class Player : MonoBehaviour
{
    public static Player Instance;

    public static event Action OnDeath = delegate { };
    private bool dead = false;
    [SerializeField]
    private Transform bossSpawnPoint;

    [SerializeField]
    private GameObject deathScreen;
    [SerializeField]
    private GameObject enemeyParent;
    void Start()
    {
        if(Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this);
        }
        if(PlayerPrefs.GetInt("spawnAtBoss") != 0)
        {
            transform.position = bossSpawnPoint.position;
            Destroy(enemeyParent);
        }
    }

    void Update()
    {
        if(Input.GetButtonDown("Fire3"))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex, LoadSceneMode.Single);
        }

        if(dead)
        {
            var playerWeapon = GetComponent<PlayerWeapon>().enabled = false; ;
            var playerVelocity = GetComponent<PlayerVelocity>().enabled = false; ;
            var playerMovement = GetComponent<PlayerMovement>().enabled = false; ;
            var playerDash = GetComponent<PlayerDash>().enabled = false; ;
            GetComponent<SpriteRenderer>().enabled = false;
            GetComponent<Rigidbody2D>().simulated = false;
            GetComponent<CircleCollider2D>().enabled = false;
        }
    }

    public void Kill()
    {
        if (!dead)
        {
            OnDeath();
            CameraShakeData.Instance.AddTrauma(0.6f);
            deathScreen.SetActive(true);
            dead = true;
            StartCoroutine(disableFog());
        }
    }
    IEnumerator disableFog()
    {
        yield return new WaitForSeconds(2f);
        Camera.main.GetComponent<ViewCone>().useFog = false;
    }
}
