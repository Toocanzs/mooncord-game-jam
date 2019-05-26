using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class ScanState : BaseState
{
    private float totalWaitTime;
    private float time;
    private Boss1 boss;

    public ScanState(Boss1 boss, float totalWaitTime) : base(boss.gameObject)
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
        if(obj.GetType() == this.GetType())
        {
            boss.scanAudioSource.Play();
            var psounds = boss.scanAudioSource.gameObject.GetComponent<BossPeridoicSounds>();
            psounds.enabled = true;
            psounds.maxTime = this.totalWaitTime;
        }
    }

    public override Type Tick()
    {
        time += Time.deltaTime;
        boss.scanAudioSource.volume = boss.scanAudioCurve.Evaluate(time/totalWaitTime);
        transform.Rotate(new Vector3(0, 0, math.cos(time*0.4f)*0.5f), Space.World);
        if (time > totalWaitTime)
        {
            time = 0f;
            return typeof(SnapState);
        }
        return GetType();
    }
}
