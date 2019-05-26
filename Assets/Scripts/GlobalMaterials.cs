using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "GlobalMaterials", menuName = "GlobalMaterials", order = 1)]
public class GlobalMaterials : SingletonScriptableObject<GlobalMaterials>
{
    public Material hiddenEnemyMaterial;
}
