using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BatteryManager : MonoBehaviour
{
    [SerializeField]
    private Boss1 boss;
    [SerializeField]
    private GameObject[] batteries;

    float time = 0f;
    [SerializeField]
    float shieldRespawnTime = 60f;
    bool shieldsActive = false;

    [SerializeField]
    private GameObject bossShield;

    private int numBatteries = 0;
    private int maxNumBatteries = 0;

    public bool allAtOnce = false;
    [SerializeField]
    private float allAtOnceTimeLimit = 10f;
    private float allAtOnceTime = 0f;

    void Start()
    {
        foreach (var battery in batteries)
        {
            battery.GetComponent<Battery>().OnBatteryDestroyed += BatteryDestoryedHandler;
        }
    }

    void OnEnable()
    {
        time = shieldRespawnTime;
    }

    private void BatteryDestoryedHandler(GameObject obj)
    {
        numBatteries--;
    }

    void Update()
    {
        if(boss.phase > 3)
        {
            allAtOnce = true;
            if (allAtOnce)
            {
                foreach (var battery in batteries)
                {
                    battery.GetComponent<Battery>().batteryProgressBarSpriteRenderer.material.SetColor("_Color", Color.red);
                }
            }
        }
        if(!shieldsActive)
        {
            time += Time.deltaTime;
            if(time > shieldRespawnTime)
            {
                SpawnBatteries();
            }
        }
        else
        {
            if(numBatteries <= 0)
            {
                bossShield.SetActive(false);
                shieldsActive = false;
                time = 0;
            }

            if (numBatteries != maxNumBatteries && allAtOnce)
            {
                allAtOnceTime += Time.deltaTime;
                if (allAtOnceTime > allAtOnceTimeLimit)
                {
                    SpawnBatteries();
                    foreach (var battery in batteries)
                    {
                        battery.GetComponent<Battery>().currentHP = GetBatteryHp();
                    }
                }
            }
            else
            {
                allAtOnceTime = 0f;
            }
        }
    }

    private void SpawnBatteries()
    {
        time = 0f;
        shieldsActive = true;
        bossShield.SetActive(true);
        foreach(var battery in batteries)
        {
            battery.GetComponent<Battery>().maxHP = GetBatteryHp();
            battery.SetActive(true);
        }
        numBatteries = batteries.Length;
        maxNumBatteries = batteries.Length;
    }

    private float GetBatteryHp()
    {
        switch(boss.phase)
        {
            case 0:
                return 10;
            case 1:
                return 10;
            case 2:
                return 20;
            case 3:
                return 20;
            default:
                return 30;
        }
    }
}
