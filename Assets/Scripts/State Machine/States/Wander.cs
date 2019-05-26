using System;
using System.Collections;
using System.Collections.Generic;
using ThetaStar;
using UnityEngine;
using Unity.Mathematics;

public class WanderState : BaseState
{
    private List<Vector3> path = null;
    private float wanderDistance;
    private int progressOnPath = 0;
    private float wanderSpeed;
    private Rigidbody2D rigidbody;
    private float pathDistanceCheck = 0.1f;
    private float wanderResetTime;
    private float wanderTime = 0f;

    public WanderState(GameObject gameObject, Rigidbody2D rigidbody, 
        float wanderDistance, float wanderSpeed, float wanderResetTime) : base(gameObject)
    {
        this.wanderDistance = wanderDistance;
        this.wanderSpeed = wanderSpeed;
        this.rigidbody = rigidbody;
        this.wanderResetTime = wanderResetTime;
    }
    public override Type Tick()
    {
        if (path == null)
        {
            Vector2 r = UnityEngine.Random.insideUnitCircle * wanderDistance;
            path = PathfindingGenerator.Instance.FindPath(transform.position, transform.position + new Vector3(r.x, r.y, 0));
            progressOnPath = 0;
            wanderTime = 0f;
        }

        if(wanderTime > wanderResetTime)
        {
            path = null;
        }
        wanderTime += Time.deltaTime;
        if (path != null && path.Count > 1 && progressOnPath < path.Count - 1)
        {
            float3 dir = math.normalize(path[progressOnPath + 1] - path[progressOnPath]);
            Debug.DrawLine(path[progressOnPath + 1], path[progressOnPath]);
            rigidbody.velocity = dir.xy * wanderSpeed;
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
