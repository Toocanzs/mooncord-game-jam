using System;
using System.Collections;
using System.Collections.Generic;
using ThetaStar;
using UnityEngine;
using Unity.Mathematics;

public class WanderState : BaseState
{
    private List<Vector3> path = null;
    private int progressOnPath = 0;
    private float pathDistanceCheck = 0.4f;
    private float wanderTime = 0f;
    private BasicEnemy basicEnemy;

    public WanderState(BasicEnemy basicEnemy) : base(basicEnemy.gameObject)
    {
        this.basicEnemy = basicEnemy;
    }
    public override Type Tick()
    {
        if (path == null)
        {
            Vector2 r = UnityEngine.Random.insideUnitCircle * basicEnemy.wanderRange;
            path = PathfindingGenerator.Instance.FindPath(transform.position, transform.position + new Vector3(r.x, r.y, 0));
            progressOnPath = 0;
            wanderTime = 0f;
        }

        if(wanderTime > basicEnemy.wanderResetTime)
        {
            path = null;
        }
        wanderTime += Time.deltaTime;
        if (path != null && path.Count > 1 && progressOnPath < path.Count - 1)
        {
            float3 dir = math.normalize(path[progressOnPath + 1] - transform.position);
            Debug.DrawLine(path[progressOnPath + 1], path[progressOnPath], Color.red);
            basicEnemy.rigidbody.velocity = dir.xy * basicEnemy.wanderSpeed;
            if (math.distancesq(transform.position, path[progressOnPath + 1]) < pathDistanceCheck * pathDistanceCheck)
            {
                progressOnPath++;
            }
        }
        else
        {
            path = null;
        }
        return null;
    }
}
