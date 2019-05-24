using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;

[RequireComponent(typeof(Camera))]
public class ViewCone : MonoBehaviour
{
    [SerializeField]
    private float viewRange = 3f;
    [SerializeField]
    private float wallBleed = 0.2f;
    new private Camera camera;
    [HideInInspector]
    public RenderTexture colorRenderTexture;
    public RenderTexture wallMask;

    [SerializeField]
    private ComputeShader computeShader;
    private int mainKernel;

    private int2 resolution;

    void Start()
    {
        //TODO: Change these buffers when resolution changes etc.
        camera = GetComponent<Camera>();
        SetupRenderTextures();

        mainKernel = computeShader.FindKernel("Main");
        if (mainKernel < 0)
            Debug.LogError("Kernel not found");
        resolution = new int2(Screen.width, Screen.height);
    }

    private void SetupRenderTextures()
    {
        colorRenderTexture = new RenderTexture(Screen.width, Screen.height, 0,
                    RenderTextureFormat.Default, RenderTextureReadWrite.Default);
        colorRenderTexture.enableRandomWrite = true;
        wallMask = new RenderTexture(Screen.width, Screen.height, 0,
            RenderTextureFormat.RHalf, RenderTextureReadWrite.Default);
        wallMask.enableRandomWrite = true;
        colorRenderTexture.Create();
        RenderBuffer[] renderBuffers = new RenderBuffer[] { colorRenderTexture.colorBuffer, wallMask.colorBuffer };
        camera.SetTargetBuffers(renderBuffers, colorRenderTexture.depthBuffer);
    }


    void Update()
    {
        if(resolution.x != Screen.width || resolution.y != Screen.height)
        {
            SetupRenderTextures();
            resolution = new int2(Screen.width, Screen.height);
        }
    }

    void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        computeShader.SetVector("_PlayerPosition", new float4(((float3)Player.Instance.transform.position).xy, 0, 0));

        Matrix4x4 screenToWorld = (camera.projectionMatrix * camera.worldToCameraMatrix).inverse;
        computeShader.SetMatrix("_ScreenToWorld", screenToWorld);

        Matrix4x4 worldToScreen = camera.projectionMatrix * camera.worldToCameraMatrix;
        computeShader.SetMatrix("_WorldToScreen", worldToScreen);

        computeShader.SetFloat("_Range", viewRange);
        computeShader.SetFloat("_WallBleed", wallBleed);

        computeShader.SetTexture(mainKernel, "ColorBuffer", colorRenderTexture);
        computeShader.SetTexture(mainKernel, "WallMask", wallMask);

        computeShader.Dispatch(mainKernel, (colorRenderTexture.width + 7) / 8,
         (colorRenderTexture.height + 7) / 8, 1);

        Graphics.Blit(colorRenderTexture, destination);
    }
}
