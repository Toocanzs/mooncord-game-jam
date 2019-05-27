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
    [SerializeField]
    private float falloff = 0.2f;
    new private Camera camera;
    [HideInInspector]
    public RenderTexture colorRenderTexture;
    [HideInInspector]
    public RenderTexture wallMask;
    [HideInInspector]
    public RenderTexture viewMask;

    [SerializeField]
    private RenderTexture fogOfWar;
    [SerializeField]
    private Material compositeMat;

    [SerializeField]
    private ComputeShader computeShader;
    private int mainKernel;

    private int2 resolution;

    public bool useFog = true;

    void Awake()
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
        colorRenderTexture = new RenderTexture(Screen.width, Screen.height, 24,
                    RenderTextureFormat.Default, RenderTextureReadWrite.Default);
        colorRenderTexture.enableRandomWrite = true;

        wallMask = new RenderTexture(Screen.width, Screen.height, 0,
            RenderTextureFormat.RHalf, RenderTextureReadWrite.Default);
        wallMask.enableRandomWrite = true;

        viewMask = new RenderTexture(Screen.width, Screen.height, 0,
            RenderTextureFormat.RHalf, RenderTextureReadWrite.Default);
        viewMask.enableRandomWrite = true;

        colorRenderTexture.Create();
        RenderBuffer[] renderBuffers = new RenderBuffer[] { colorRenderTexture.colorBuffer, wallMask.colorBuffer };
        camera.SetTargetBuffers(renderBuffers, colorRenderTexture.depthBuffer);
    }


    void Update()
    {
        if (resolution.x != Screen.width || resolution.y != Screen.height)
        {
            SetupRenderTextures();
            resolution = new int2(Screen.width, Screen.height);
        }
    }

    void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        int downscale = (int)math.round(((float)Screen.width)/1920);

        computeShader.SetVector("_PlayerPosition", new float4(((float3)Player.Instance.transform.position).xy, 0, 0));

        Matrix4x4 screenToWorld = (camera.projectionMatrix * camera.worldToCameraMatrix).inverse;
        computeShader.SetMatrix("_ScreenToWorld", screenToWorld);

        Matrix4x4 worldToScreen = camera.projectionMatrix * camera.worldToCameraMatrix;
        computeShader.SetMatrix("_WorldToScreen", worldToScreen);

        computeShader.SetFloat("_Range", viewRange);
        computeShader.SetFloat("_WallBleed", wallBleed);
        computeShader.SetFloat("_Falloff", falloff);

        RenderTexture rt = RenderTexture.active;
        RenderTexture.active = viewMask;
        GL.Clear(true, true, Color.white);
        RenderTexture.active = rt;

        //Downscale to save performance
        RenderTexture viewMaskTemp = RenderTexture.GetTemporary(new RenderTextureDescriptor(viewMask.width>> downscale, viewMask.height>> downscale, viewMask.format));
        RenderTexture wallMaskTemp = RenderTexture.GetTemporary(new RenderTextureDescriptor(wallMask.width>> downscale, wallMask.height>> downscale, wallMask.format));
        viewMaskTemp.enableRandomWrite = true;
        wallMaskTemp.enableRandomWrite = true;
        Graphics.Blit(viewMask, viewMaskTemp);
        Graphics.Blit(wallMask, wallMaskTemp);

        computeShader.SetTexture(mainKernel, "ViewMask", viewMaskTemp);
        computeShader.SetTexture(mainKernel, "WallMask", wallMaskTemp);

        computeShader.Dispatch(mainKernel, (viewMask.width + 7) / 8,
         (viewMask.height + 7) / 8, 1);

        Graphics.Blit(viewMaskTemp, viewMask);

        RenderTexture.ReleaseTemporary(wallMaskTemp);

        float2 FOWcameraPos = ((float3)FogOfWarCamera.Instance.transform.position).xy;
        float2 cameraPos = ((float3)camera.transform.position).xy;
        float2 FOWsize = new float2(FogOfWarCamera.Instance.orthographicSize * FogOfWarCamera.Instance.aspect, FogOfWarCamera.Instance.orthographicSize);
        float2 FOWstart = FOWcameraPos - FOWsize;
        float2 FOWend = FOWcameraPos + FOWsize;
        float2 cameraSize = new float2(camera.orthographicSize * camera.aspect, camera.orthographicSize);

        float2 cameraStart = ((float3)camera.ScreenToWorldPoint(new Vector3(0,0,0))).xy;
        float2 cameraEnd = ((float3)camera.ScreenToWorldPoint(new Vector3(camera.pixelWidth, camera.pixelHeight, 0))).xy;
        float cameraAngle = camera.transform.rotation.eulerAngles.z * Mathf.Deg2Rad;

        compositeMat.SetVector("cameraStartEnd", new float4(cameraStart, cameraEnd));
        compositeMat.SetVector("FOWStartEnd", new float4(FOWstart, FOWend));
        compositeMat.SetFloat("cameraAngle", cameraAngle);
        compositeMat.SetVector("FOWsize", new float4(FOWsize, 0, 0));
        compositeMat.SetVector("cameraSize", new float4(cameraSize, 0, 0));

        compositeMat.SetTexture("_FogOfWar", FogOfWarCamera.Instance.activeTexture);
        compositeMat.SetTexture("_ViewMask", viewMaskTemp);
        compositeMat.SetInt("useFog", useFog ? 1 : 0);
        
        Graphics.Blit(colorRenderTexture, destination, compositeMat);
        RenderTexture.ReleaseTemporary(viewMaskTemp);
    }
}
