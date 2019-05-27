using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomLaser : MonoBehaviour
{
    [SerializeField]
    private GameObject laser;
    [SerializeField]
    private GameObject horizontal;
    [SerializeField]
    private GameObject vertical;
    [SerializeField]
    private float timeBetweenShots = 0.05f;
    private float vertTime;
    private float horizontalTime;
    private int vertFired = int.MaxValue;
    private int horizontalFired = int.MaxValue;

    private List<Transform> horizontalTransforms = new List<Transform>();
    private List<Transform> verticalTransforms = new List<Transform>();
    void Start()
    {
        foreach (Transform child in horizontal.transform)
        {
            horizontalTransforms.Add(child);
        }
        foreach (Transform child in vertical.transform)
        {
            verticalTransforms.Add(child);
        }
    }
    void OnEnable()
    {
        vertTime = 0f;
        horizontalTime = 0f;
    }
    void Update()
    {
        if(vertFired < verticalTransforms.Count && vertTime > timeBetweenShots)
        {
            Fire(verticalTransforms[vertFired]);
            vertTime = 0f;
            vertFired++;
        }

        if (horizontalFired < horizontalTransforms.Count && horizontalTime > timeBetweenShots)
        {
            Fire(horizontalTransforms[horizontalFired]);
            horizontalTime = 0f;
            horizontalFired++;
        }
        horizontalTime += Time.deltaTime;
        vertTime += Time.deltaTime;
    }

    void OnDrawGizmosSelected()
    {
        foreach (Transform child in horizontal.transform)
        {
            Gizmos.DrawRay(child.position, child.right);
        }

        foreach (Transform child in vertical.transform)
        {
            Gizmos.DrawRay(child.position, child.right);
        }
    }

    public void FireHorizontal()
    {
        horizontalFired = 0;
    }

    public void FireVertical()
    {
        vertFired = 0;
    }

    private void Fire(Transform point)
    {
        GameObject go = Instantiate(laser, point.position, point.rotation);
        go.GetComponent<LaserShot>().trauma = 0.02f;
        go.GetComponent<AudioSource>().volume = 0.2f;
    }
}
