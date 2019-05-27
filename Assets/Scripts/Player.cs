using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public static Player Instance;

    public static event Action OnDeath = delegate { };
    private bool dead = false;

    [SerializeField]
    private GameObject deathScreen;
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
    }

    void Update()
    {
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
