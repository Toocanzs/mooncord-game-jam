using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmgBurst : MonoBehaviour
{
    public int shotNum = 10;
    [SerializeField]
    private GameObject bullet;
    [SerializeField]
    private AudioClip audioClip;
    public float angle = 90;
    [SerializeField]
    private float totalShotTime = 0.8f;
    private float time = 0f;
    private int shot = 0;
    [SerializeField]
    private float totalTrauma = 0.3f;
    void OnEnable()
    {
        shot = 0;
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
        Instantiate(bullet, transform.position, Quaternion.Euler(transform.rotation.eulerAngles +
                                    new Vector3(0, 0, (i - shotNum / 2) * (angle / shotNum))));
        CameraShakeData.Instance.AddTrauma((1f/ (float)shotNum)*totalTrauma);
        shot++;
    }
}
