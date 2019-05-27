using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorTrigger : MonoBehaviour
{
    [SerializeField]
    private GameObject[] onTriggerObjects;
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<Player>() != null)
        {
            foreach(var obj in onTriggerObjects)
                obj.SetActive(true);
            PlayerPrefs.SetInt("spawnAtBoss", 1);
            Destroy(gameObject);
        }
    }
}
