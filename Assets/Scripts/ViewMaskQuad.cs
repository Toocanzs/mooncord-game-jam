using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[DefaultExecutionOrder(5000)]
[RequireComponent(typeof(MeshRenderer))]
public class ViewMaskQuad : MonoBehaviour
{
    private RenderTexture viewMask;
    private ViewCone viewCone;
    new private Camera camera;
    void Start()
    {
        camera = Camera.main;
        viewCone = camera.GetComponent<ViewCone>();
        viewMask = viewCone.viewMask;
        GetComponent<MeshRenderer>().material.SetTexture("_MainTex", viewMask);
    }
    void Update()
    {
        if(viewCone.viewMask != viewMask)
        {
            viewMask = viewCone.viewMask;
            GetComponent<MeshRenderer>().material.SetTexture("_MainTex", viewMask);
        }
        transform.localScale = new Vector3(camera.orthographicSize * 2* camera.aspect, camera.orthographicSize*2, 1);
    }
}
