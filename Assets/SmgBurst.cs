using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmgBurst : MonoBehaviour
{
    public int shotNum = 10;
    [SerializeField]
    private GameObject dodgeableBullet;
    [SerializeField]
    private GameObject undodgeableBullet;
    [SerializeField]
    private AudioClip audioClip;
    public float angle = 90;
    [SerializeField]
    private float totalShotTime = 0.8f;
    private float time = 0f;
    private int shot = 0;
    [SerializeField]
    private float totalTrauma = 0.3f;

    public float undodgeableChange = 0f;
    private int undodgeableStart = -1;
    private int undodgeableEnd = -1;
    void OnEnable()
    {
        shot = 0;

        int numDodgeable = 0;
        for (int i = 0; i < shotNum; i++)
        {
            if(UnityEngine.Random.Range(0f, 1f) < undodgeableChange)
                numDodgeable++;
        }
        if (numDodgeable == 0)
        {
            undodgeableStart = -1;
            undodgeableEnd = -1;
            return;
        }
        undodgeableStart = UnityEngine.Random.Range(0, (shotNum - numDodgeable));
        undodgeableEnd = undodgeableStart + numDodgeable;
    }

    void Update()
    {
        if (shot >= shotNum)
        {
            Destroy(gameObject);
            return;
        }
        float delta = totalShotTime / shotNum;
        if (time > delta)
        {
            if(shot == 0)
                AudioPlayer.Instance.PlayOneShot(audioClip);
            if (totalShotTime == 0f)
            {
                while (totalShotTime == 0f && shot < shotNum)
                {
                    Shoot(shot);
                }
                Destroy(gameObject);
                return;
            }
            else
            {
                while(time > delta)
                {
                    Shoot(shot);
                    time -= delta;
                }
            }
        }
        time += Time.deltaTime;
    }

    private void Shoot(int i)
    {
        if (i >= undodgeableStart && i < undodgeableEnd)
        {
            Instantiate(undodgeableBullet, transform.position, Quaternion.Euler(transform.rotation.eulerAngles +
                                        new Vector3(0, 0, (i - shotNum / 2) * (angle / shotNum))));
        }
        else
        {
            Instantiate(dodgeableBullet, transform.position, Quaternion.Euler(transform.rotation.eulerAngles +
                                        new Vector3(0, 0, (i - shotNum / 2) * (angle / shotNum))));
        }
        CameraShakeData.Instance.AddTrauma((1f/ (float)shotNum)*totalTrauma);
        shot++;
    }
}
