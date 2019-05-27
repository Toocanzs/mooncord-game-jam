using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class ChaseState : BaseState
{
    private Boss1 boss;
    private float time = 0f;
    private float nextAttackTime = 0f;
    public ChaseState(Boss1 boss) : base(boss.gameObject)
    {
        this.boss = boss;
        boss.stateMachine.OnStateChanged += StateChangeHandler;
        nextAttackTime = boss.GetTimeBetweenAttacks();
    }

    private void StateChangeHandler(BaseState obj)
    {
        if (obj.GetType() == this.GetType())
        {

        }
    }

    public override Type Tick()
    {
        if (Player.Instance.enabled == false)
            return GetType();
        float2 dir = math.normalizesafe(((float3)Player.Instance.transform.position).xy - ((float3)transform.position).xy);
        transform.position = Vector3.MoveTowards(transform.position, Player.Instance.transform.position, Time.deltaTime * boss.bossMoveSpeed);
        transform.up = new float3(-dir, 0);
        boss.UpdatePhase();
        
        if (time > nextAttackTime)
        {
            time = 0f;
            nextAttackTime = boss.GetTimeBetweenAttacks() + boss.ChooseAttack();
        }
        time += Time.deltaTime;
        return GetType();
    }
}
