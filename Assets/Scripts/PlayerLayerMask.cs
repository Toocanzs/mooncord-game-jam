using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "PlayerLayerMask", menuName = "PlayerLayerMask", order = 1)]
public class PlayerLayerMask : SingletonScriptableObject<PlayerLayerMask>
{
    public LayerMask layerMask;
}
