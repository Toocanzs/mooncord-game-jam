using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;
using UnityEngine.UI;

public class BossHealthBar : MonoBehaviour
{
    [SerializeField]
    private Boss1 boss;
    private float maxWidth = 5f;
    private Vector3 startPos;
    void Start()
    {
        startPos = transform.localPosition;
    }

    void Update()
    {
        float p = math.saturate(boss.currentHp / boss.maxHp);
        float w = p * maxWidth;
        transform.localPosition = new Vector3((w/2f) - (maxWidth/2),0,0);
        transform.localScale = new Vector3(math.saturate(boss.currentHp / boss.maxHp), 1,1);
    }
}
