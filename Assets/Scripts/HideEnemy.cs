using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HideEnemy : MonoBehaviour
{
    void Start()
    {
        GetComponent<SpriteRenderer>().sharedMaterial = GlobalMaterials.Instance.hiddenEnemyMaterial;
    }

    void Update()
    {
        
    }
}
