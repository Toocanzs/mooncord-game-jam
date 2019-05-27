﻿using System.Collections;
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

    private IEnumerator Horizontal()
    {
        for(int i = 0; i < horizontalTransforms.Count; i++)
        {
            Fire(horizontalTransforms[i]);
            yield return new WaitForSeconds(timeBetweenShots);
        }
    }

    private IEnumerator HorizontalSplit()
    {
        for (int i = 0; i < horizontalTransforms.Count; i++)
        {
            if(i%2 == 0)
                Fire(horizontalTransforms[i/2]);
            else
                Fire(horizontalTransforms[horizontalTransforms.Count - 1 - (i/2)]);
            yield return new WaitForSeconds(timeBetweenShots);
        }
    }

    private IEnumerator VerticalSplit()
    {
        for (int i = 0; i < verticalTransforms.Count; i++)
        {
            if (i % 2 == 0)
                Fire(verticalTransforms[i / 2]);
            else
                Fire(verticalTransforms[verticalTransforms.Count - 1 - (i / 2)]);
            yield return new WaitForSeconds(timeBetweenShots*2f);
        }
    }


    private IEnumerator Vertical()
    {
        for (int i = 0; i < verticalTransforms.Count; i++)
        {
            Fire(verticalTransforms[i]);
            yield return new WaitForSeconds(timeBetweenShots);
        }
    }

    public void FireHorizontal()
    {
        StartCoroutine(Horizontal());
    }

    public void FireVertical()
    {
        StartCoroutine(Vertical());
    }

    public void FireHorizontalSplit()
    {
        StartCoroutine(HorizontalSplit());
        
    }

    public void FireVerticalSplit()
    {
        StartCoroutine(VerticalSplit());
    }

    private void Fire(Transform point)
    {
        GameObject go = Instantiate(laser, point.position, point.rotation);
        go.GetComponent<LaserShot>().trauma = 0.02f;
        go.GetComponent<AudioSource>().volume = 0.2f;
    }
}
