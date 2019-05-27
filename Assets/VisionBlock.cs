using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VisionBlock : MonoBehaviour
{
    [SerializeField]
    private AnimationCurve animationCurve;
    private float time = 0f;
    [SerializeField]
    private float maxTime = 0f;
    [SerializeField]
    private float maxHeight = 5f;

    private Vector3 startSize;

    void OnEnable()
    {
        time = 0f;
        startSize = transform.localScale;
    }
    void Update()
    {
        float t = time / maxTime;
        if (t > 1f)
        {
            //Destroy(this);
            return;
        }
        transform.localScale = new Vector3(startSize.x, animationCurve.Evaluate(t) * maxHeight, startSize.z);
        time += Time.deltaTime;
    }
}
