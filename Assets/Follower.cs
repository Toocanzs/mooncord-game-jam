using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Follower : MonoBehaviour
{
    [SerializeField]
    private Transform follow;
    [SerializeField]
    private Vector3 offset = Vector3.zero;
    void Start()
    {
        
    }
    void LateUpdate()
    {
        transform.position = follow.position + offset;
    }
}
