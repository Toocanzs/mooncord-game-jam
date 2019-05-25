using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using static Unity.Mathematics.math;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraShake : MonoBehaviour
{
    [SerializeField]
    private float traumaDecay = 1f;
    private CameraShakeData cameraShakeData;

    [SerializeField]
    private float2 maxTranslationalOffset = 1;
    [SerializeField]
    private float2 translationNoiseFrequency = 1;
    private float2 translationSeeds;

    [SerializeField]
    private float maxRotationOffset = 1f;
    [SerializeField]
    private float rotationNoiseFrequency = 1f;
    [SerializeField]
    private Vector3 cameraOffset = Vector3.zero;
    private float rotationSeed;

    new private Camera camera;


    void Start()
    {
        camera = GetComponent<Camera>();
        cameraShakeData = CameraShakeData.Instance;
        translationSeeds = float2(UnityEngine.Random.Range(-100000, 100000), UnityEngine.Random.Range(-100000, 100000));
        rotationSeed = UnityEngine.Random.Range(-100000, 100000);
        cameraShakeData.Trauma = 0f;
    }

    void Update()
    {
        float shake = cameraShakeData.Trauma * cameraShakeData.Trauma;
        transform.position = transform.parent.position + GetTranslationOffset(shake) + cameraOffset;
        transform.rotation = GetShakeRotation(shake);
        cameraShakeData.AddTrauma(-Time.deltaTime * traumaDecay);
    }

    private Quaternion GetShakeRotation(float shake)
    {
        float noise = -1 + 2 * Mathf.PerlinNoise(Time.time * rotationNoiseFrequency, rotationSeed);
        noise *= maxRotationOffset * shake * 15f;
        float3 euler = transform.parent.eulerAngles;
        return Quaternion.Euler(euler.x, euler.y, euler.z + noise);
    }

    private Vector3 GetTranslationOffset(float shake)
    {
        float2 noise = float2(
            -1 + 2 * Mathf.PerlinNoise(Time.time * translationNoiseFrequency.x, translationSeeds.x),
            -1 + 2 * Mathf.PerlinNoise(Time.time * translationNoiseFrequency.y, translationSeeds.y)
            );

        float2 shakeOffset = noise * maxTranslationalOffset * shake;
        return float3(shakeOffset.x, shakeOffset.y, 0);
    }
}
