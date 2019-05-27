using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;
using UnityEngine.UI;

public class ShieldBar : MonoBehaviour
{
    [SerializeField]
    private BatteryManager batteryManager;
    private float maxWidth = 5f;
    private Vector3 startPos;
    void Start()
    {
        startPos = transform.localPosition;
    }

    void LateUpdate()
    {
        float p = math.saturate(batteryManager.time / batteryManager.shieldRespawnTime);
        float w = p * maxWidth;
        transform.localPosition = new Vector3((w/2f) - (maxWidth/2),0,0);
        transform.localScale = new Vector3(p, 1,1);
    }
}
