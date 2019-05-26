using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class SkipTip : MonoBehaviour
{
    [SerializeField]
    private AnimationCurve animationCurve;
    [SerializeField]
    private float totalTime;
    private float time;

    private TextMeshProUGUI text;

    void Start()
    {
        text = GetComponent<TextMeshProUGUI>();
    }

    void Update()
    {
        time += Time.deltaTime;
        text.color = new Color(1,1,1,animationCurve.Evaluate(time/totalTime));
        if(time > totalTime || Input.GetButtonDown("Fire2"))
        {
            Destroy(transform.parent.gameObject);
        }
    }
}
