using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Unity.Mathematics.math;

[CreateAssetMenu(fileName = "CameraShakeData", menuName = "CameraShakeData", order = 1)]
public class CameraShakeData : SingletonScriptableObject<CameraShakeData>
{
    private float trauma;
    public float Trauma
    {
        get
        {
            return trauma;
        }
        set
        {
            trauma = saturate(value);
        }
    }

    public void AddTrauma(float stress)
    {
        trauma = saturate(Trauma + stress);
    }
}
