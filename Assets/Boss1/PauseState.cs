using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseState : BaseState
{
    private float totalWaitTime;
    private float time;
    private Type nextState;

    public PauseState(GameObject gameObject, float totalWaitTime, Type nextState) : base(gameObject)
    {
        this.totalWaitTime = totalWaitTime;
        this.nextState = nextState;
    }

    public void SetTime(float totalWaitTime, Type nextState)
    {
        this.totalWaitTime = totalWaitTime;
        this.nextState = nextState;
    }

    public override Type Tick()
    {
        time += Time.deltaTime;
        if(time > totalWaitTime)
        {
            time = 0f;
            return nextState;
        }
        return GetType();
    }
}
