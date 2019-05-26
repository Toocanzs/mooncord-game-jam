using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class SnapState : BaseState
{
    public float totalWaitTime;
    public float time;
    private Quaternion startRotation;
    private Boss1 boss;
    private bool played = false;

    public SnapState(Boss1 boss, float totalWaitTime) : base(boss.gameObject)
    {
        this.totalWaitTime = totalWaitTime;
        this.boss = boss;
        boss.stateMachine.OnStateChanged += StateChangeHandler;
    }

    public void SetTime(float totalWaitTime, Type nextState)
    {
        this.totalWaitTime = totalWaitTime;
    }

    private void StateChangeHandler(BaseState obj)
    {
        if (obj.GetType() == this.GetType())
        {
            boss.audioSource.PlayOneShot(boss.scanSnapClip);
            startRotation = transform.rotation;
            boss.hitSnapState = true;
        }
    }

    public override Type Tick()
    {
        time += Time.deltaTime;
        float2 dir = math.normalize(((float3)Player.Instance.transform.position).xy - ((float3)transform.position).xy);
        float angle = math.degrees(math.atan2(dir.y, dir.x)) + 90;
        transform.rotation = Quaternion.Slerp(startRotation,
            Quaternion.Euler(new Vector3(0,0,angle)),
            boss.scanSnapCurve.Evaluate((time/totalWaitTime)));
        if(time > 6f && !played)
        {
            played = true;
            boss.audioSource.PlayOneShot(boss.scanTargetFound);
        }
        if(time > totalWaitTime)
        {
            time = 0f;
            boss.shieldGameObject.SetActive(false);
            return null;
        }
        return GetType();
    }
}
