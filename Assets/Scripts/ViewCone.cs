using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[DefaultExecutionOrder(10)]
[RequireComponent(typeof(Camera))]
public class ViewCone : MonoBehaviour
{
    new private Camera camera;
    [HideInInspector]
    public RenderTexture colorRenderTexture;
    [HideInInspector]
    public RenderTexture wallMask;

    [SerializeField]
    private ComputeShader computeShader;
    private int mainKernel;

    void Start()
    {
        //TODO: Change these buffers when resolution changes etc.
        camera = GetComponent<Camera>();
        colorRenderTexture = new RenderTexture(camera.pixelWidth, camera.pixelHeight, 0,
            RenderTextureFormat.Default, RenderTextureReadWrite.Default);
        wallMask = new RenderTexture(camera.pixelWidth, camera.pixelHeight, 0,
            RenderTextureFormat.R8, RenderTextureReadWrite.Default);
        colorRenderTexture.Create();
        RenderBuffer[] renderBuffers = new RenderBuffer[] { colorRenderTexture.colorBuffer, wallMask.colorBuffer };
        camera.SetTargetBuffers(renderBuffers, colorRenderTexture.depthBuffer);

        mainKernel = computeShader.FindKernel("Main");
    }

    void Update()
    {
        
    }

    void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        Graphics.Blit(colorRenderTexture, destination);
    }
}
