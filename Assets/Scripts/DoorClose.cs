using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;

public class DoorClose : MonoBehaviour
{
    private float percent = 0f;
    [SerializeField]
    private float closeTime = 1f;
    [SerializeField]
    private AnimationCurve curve;
    private float time = 0f;
    private int hits = 0;
    void Start()
    {
        
    }
    //0.3/0.73

    void OnEnable()
    {
        percent = 0f;
        time = 0f;
        hits = 0;
    }

    void Update()
    {
        float t = math.saturate((time * time) / closeTime);
        time += Time.deltaTime;
        if (t > 0.3 && hits < 1)
            HitWall(0.5f);
        if (t > 0.73 && hits < 2)
            HitWall(0.25f);
        if (t >= 1 && hits < 3)
            HitWall(0.125f);
        percent = curve.Evaluate(t);
        transform.localPosition = new float3(-0.5f + (1-(percent/2)), 0, 0);
        transform.localScale = new float3(percent, 1,1);
    }

    private void HitWall(float trauma)
    {
        hits++;
        CameraShakeData.Instance.AddTrauma(trauma);
    }
}
