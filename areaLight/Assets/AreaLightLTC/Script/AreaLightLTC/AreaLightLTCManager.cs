using System;
using System.Collections;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using System.Collections.Generic;
using UnityEngine;

public class AreaLightLTCManager
{
    private static AreaLightLTCManager instance;
    public static AreaLightLTCManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new AreaLightLTCManager();
            }
            return instance;
        }
    }
    public List<AreaLightLTC> areaLightList = new List<AreaLightLTC>();
    
    private int MaxareaLightCount = 8;
    private int panelLightVertsID = Shader.PropertyToID("_LightVerts");
    private int panelLocalMatrixID = Shader.PropertyToID("_panelLocalMatrix");
    private int panelLightPosID = Shader.PropertyToID("_panelLightPos");
    private int panelLightColorID = Shader.PropertyToID("_panelLightColor");
    private int panelLightInstensityID = Shader.PropertyToID("_panelLightInstensity");
    private int panelLightRoughnessID = Shader.PropertyToID("_panelLightRoughness");

    public readonly float[,] offsets = 
        new float[4, 2] 
        { 
            { 1, 1 }, 
            { 1, -1 }, 
            { -1, -1 }, 
            { -1, 1 } 
        };

    /// <summary>
    /// add函数，需要进行修改shaderID
    /// </summary>
    public void Add(AreaLightLTC areaLight)
    {
        //初始化buffer
        if(areaLightList.Count > MaxareaLightCount) Debug.Log("当前场景面光源数量超过8");
        areaLightList.Add(areaLight);
    }

    /// <summary>
    /// remove函数，需要进行修改shaderID
    /// </summary>
    public void Remove(AreaLightLTC areaLight)
    {
        if (areaLightList.IndexOf(areaLight) >= 0)
        {
            areaLightList.Remove(areaLight);
        }
    }
    
    /// <summary>
    /// 设置datas
    /// </summary>
    /// <param name="areaLight"></param>
    /// <param name="data"></param>
    public void SetDatas(AreaLightLTC areaLight, CommandBuffer buffer)
    {
        buffer.SetGlobalMatrix(panelLocalMatrixID, areaLight.transform.worldToLocalMatrix);
        Matrix4x4 lightVerts = new Matrix4x4();
        for (int i = 0; i < 4; i++)
        {
            lightVerts.SetRow(i, areaLight.transform.TransformPoint(new Vector3(areaLight.lightSize.x * offsets[i, 0], 
                areaLight.lightSize.y * offsets[i, 1], 0.01f) * 0.5f));
        }
        areaLight.SetUpLut(buffer);
        buffer.SetGlobalMatrix(panelLightVertsID, lightVerts);
        buffer.SetGlobalVector(panelLightPosID, areaLight.transform.position);
        buffer.SetGlobalVector(panelLightColorID, areaLight.lightColor);
        buffer.SetGlobalFloat(panelLightInstensityID, areaLight.lightIntensity);
        buffer.SetGlobalFloat(panelLightRoughnessID, areaLight.lightRoughness);
    }
}
