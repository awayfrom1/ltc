using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class AreaLightManager
{
    private static AreaLightManager instance;
    public static AreaLightManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new AreaLightManager();
            }
            return instance;
        }
    }
    public List<AreaLight> areaLightList = new List<AreaLight>();
    
    private int MaxareaLightCount = 8;
    private int panelLocalMatrixID = Shader.PropertyToID("_panelLocalMatrix");
    private int panelLightPosID = Shader.PropertyToID("_panelLightPos");
    private int panelLightWorldPNormalID = Shader.PropertyToID("_panelLightWorldPNormal");
    private int panelLightifTranverseID = Shader.PropertyToID("_panelLightifTranverse");
    private int panelLightRangeID = Shader.PropertyToID("_panelLightRange");
    private int panellightAttenutionID = Shader.PropertyToID("_panellightAttenution");
    private int panelLightColorID = Shader.PropertyToID("_panelLightColor");
    private int panelLightInstensityID = Shader.PropertyToID("_panelLightInstensity");
    private int panelLocalScaleID = Shader.PropertyToID("_panelLocalScale");

    /// <summary>
    /// add函数，需要进行修改shaderID
    /// </summary>
    public void Add(AreaLight areaLight)
    {
        //初始化buffer
        if(areaLightList.Count > MaxareaLightCount) Debug.Log("当前场景面光源数量超过8");
        areaLightList.Add(areaLight);
    }

    /// <summary>
    /// remove函数，需要进行修改shaderID
    /// </summary>
    public void Remove(AreaLight areaLight)
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
    public void SetDatas(CommandBuffer buffer, AreaLight areaLight)
    {
        buffer.SetGlobalMatrix(panelLocalMatrixID, areaLight.transform.worldToLocalMatrix);
        buffer.SetGlobalVector(panelLightPosID, areaLight.transform.position);
        buffer.SetGlobalVector(panelLightWorldPNormalID, areaLight.transform.TransformVector(new Vector3(0, 1, 0)));
        buffer.SetGlobalFloat(panelLightifTranverseID, areaLight.transform.forward.y > 0 ? 1 : 0);
        buffer.SetGlobalVector(panelLightRangeID, areaLight.lightRange);
        buffer.SetGlobalVector(panellightAttenutionID, areaLight.lightAttenution);
        buffer.SetGlobalVector(panelLightColorID, areaLight.lightColor);
        buffer.SetGlobalFloat(panelLightInstensityID, areaLight.lightInstensity);
        buffer.SetGlobalFloat(panelLocalScaleID, Mathf.Max(1, areaLight.transform.localScale.x + areaLight.transform.localScale.y + areaLight.transform.localScale.z) / 6.0f);
    }
}

