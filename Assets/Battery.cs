using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(SpriteRenderer))]
public class Battery : MonoBehaviour
{
    public float maxHP = 30;
    public float currentHP;
    [SerializeField]
    private AudioClip hitSound;
    public event Action<GameObject> OnBatteryDestroyed = delegate { };

    public SpriteRenderer batteryProgressBarSpriteRenderer;
    [SerializeField]
    private GameObject shield;

    [SerializeField]
    private GameObject deathObject;

    void OnEnable()
    {
        currentHP = maxHP;
        shield.transform.rotation = Quaternion.Euler(new Vector3(0,0,UnityEngine.Random.Range(0f, 360f)));
    }
    void Update()
    {
        if(currentHP < 0f)
        {
            OnBatteryDestroyed(gameObject);
            var go = Instantiate(deathObject, transform.position, Quaternion.identity);
            Destroy(go, 6f);
            gameObject.SetActive(false);
        }
        batteryProgressBarSpriteRenderer.material.SetFloat("percent", currentHP/maxHP);
    }

    public void Hit(float damage)
    {
        currentHP -= damage;
        AudioPlayer.Instance.PlayOneShot(hitSound, 0.5f);
    }
}
