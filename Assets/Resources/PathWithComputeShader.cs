using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DualPantoFramework;

public class PathWithComputeShader : MonoBehaviour
{
    public ComputeShader computeShader;
    public RenderTexture renderTexture;
    UpperHandle upperHandle;
    // Start is called before the first frame update

    Vector3 lastPos;
    const int HEIGHT = 1024;
    const int WIDTH = HEIGHT * 2;

    void Start()
    {
        upperHandle = GameObject.Find("Panto").GetComponent<UpperHandle>();
    }
    
    void Update()
    {
        if (renderTexture == null)
        {
            renderTexture = new RenderTexture(WIDTH, HEIGHT, 24, RenderTextureFormat.ARGB32);
            renderTexture.enableRandomWrite = true;
            renderTexture.Create();

            GetComponent<MeshRenderer>().material.mainTexture = renderTexture;
        }

        computeShader.SetTexture(0, "Result", renderTexture);
        var bounds = GetComponent<MeshFilter>().mesh.bounds;
        Vector3 position = upperHandle.HandlePosition(new Vector3(0, 0, 0)) - transform.TransformPoint(bounds.min);

        if (lastPos != null) {
            computeShader.SetBool("moving", true);
            computeShader.SetInt("width", WIDTH);
            computeShader.SetInt("height", HEIGHT);
            computeShader.SetFloat("last_x", 1.0f - lastPos.x / bounds.size.x / transform.localScale.x);
            computeShader.SetFloat("last_y", 1.0f - lastPos.z / bounds.size.z / transform.localScale.z);
            computeShader.SetFloat("x", 1.0f - position.x / bounds.size.x / transform.localScale.x);
            computeShader.SetFloat("y", 1.0f - position.z / bounds.size.z / transform.localScale.z);
            computeShader.Dispatch(0, renderTexture.width / 8, renderTexture.height / 8, 1);
            lastPos = position;
        }
    }
}

#if false
using UnityEngine;
using UnityEngine.Experimental.Rendering;
using ComputeShaderUtility;

public class Simulation : MonoBehaviour
{

    const int updateKernel = 0;
    const int diffuseMapKernel = 1;

    public ComputeShader compute;


    [Header("Display Settings")]
    public bool showAgentsOnly;
    public FilterMode filterMode = FilterMode.Point;
    public GraphicsFormat format = ComputeHelper.defaultGraphicsFormat;


    [SerializeField, HideInInspector] protected RenderTexture trailMap;
    [SerializeField, HideInInspector] protected RenderTexture diffusedTrailMap;
    [SerializeField, HideInInspector] protected RenderTexture displayTexture;

    Texture2D colourMapTexture;

    protected virtual void Start()
    {
        Init();
        transform.GetComponentInChildren<MeshRenderer>().material.mainTexture = displayTexture;
    }


    void Init()
    {

        // Create render textures
        ComputeHelper.CreateRenderTexture(ref trailMap, settings.width, settings.height, filterMode, format);
        ComputeHelper.CreateRenderTexture(ref diffusedTrailMap, settings.width, settings.height, filterMode, format);
        ComputeHelper.CreateRenderTexture(ref displayTexture, settings.width, settings.height, filterMode, format);

        compute.SetInt("width", settings.width);
        compute.SetInt("height", settings.height);
    }

    void Update()
    {

        // Assign textures
        compute.SetTexture(updateKernel, "TrailMap", trailMap);
        compute.SetTexture(diffuseMapKernel, "TrailMap", trailMap);
        compute.SetTexture(diffuseMapKernel, "DiffusedTrailMap", diffusedTrailMap);

        // Assign settings
        compute.SetFloat("deltaTime", Time.deltaTime);
        compute.SetFloat("time", Time.time);

        compute.SetFloat("trailWeight", settings.trailWeight);
        compute.SetFloat("decayRate", settings.decayRate);
        compute.SetFloat("diffuseRate", settings.diffuseRate);


        ComputeHelper.Dispatch(compute, settings.numAgents, 1, 1, kernelIndex: updateKernel);
        ComputeHelper.Dispatch(compute, settings.width, settings.height, 1, kernelIndex: diffuseMapKernel);

        ComputeHelper.CopyRenderTexture(diffusedTrailMap, trailMap);

        ComputeHelper.CopyRenderTexture(trailMap, displayTexture);

    }
}
#endif
