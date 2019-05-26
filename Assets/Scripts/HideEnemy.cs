using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HideEnemy : MonoBehaviour
{
    [SerializeField]
    private Material materialOverride;
    void Start()
    {
        if(materialOverride == null)
            GetComponent<SpriteRenderer>().sharedMaterial = GlobalMaterials.Instance.hiddenEnemyMaterial;
        else
            GetComponent<SpriteRenderer>().sharedMaterial = materialOverride;
    }

    void Update()
    {
        
    }
}
