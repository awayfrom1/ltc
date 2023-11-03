using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

[ExecuteInEditMode]
public class AreaLight : MonoBehaviour
{
    public Color lightColor = Color.white;
    public Vector3 lightRange = new Vector3(5, 20 ,0);
    public Vector3 lightAttenution = new Vector3(1, 0, 2);
    public float lightInstensity = 1;

    private Material lightMat;
    private Renderer renderer;
    private MaterialPropertyBlock prop;
    private int _panelLightColorID = Shader.PropertyToID("_panelLightColor");
    private int _panelLightFaceInstensityID = Shader.PropertyToID("_panelLightFaceInstensity");

    private void OnEnable()
    {
        AreaLightManager.Instance.Add(this);
        prop = new MaterialPropertyBlock();
        renderer = this.GetComponent<MeshRenderer>();
    }
    
    private void Update()
    {
        prop.SetColor(_panelLightColorID, lightColor);
        prop.SetFloat(_panelLightFaceInstensityID, lightInstensity);
        renderer.SetPropertyBlock(prop);
    }

    private void OnDisable()
    {
        AreaLightManager.Instance.Remove(this);
        prop = null;
        renderer = null;
    }

#if UNITY_EDITOR
    private Vector3[] planeGimoz = new []
    {
        new Vector3(-5, 0, -5),
        new Vector3(-5, 0, 5),
        new Vector3(5, 0, 5),
        new Vector3(5, 0, -5),
        new Vector3(0, 0, 0),
        new Vector3(0, -2.5f, 0),
    };
    
    private void OnDrawGizmos()
    {
        // Gizmos.DrawLine(planeGimoz[0], planeGimoz[1]);
        // Gizmos.DrawLine(planeGimoz[1], planeGimoz[2]);
        // Gizmos.DrawLine(planeGimoz[2], planeGimoz[3]);
        // Gizmos.DrawLine(planeGimoz[3], planeGimoz[0]);
        Gizmos.DrawLine(this.transform.TransformPoint(planeGimoz[0]), this.transform.TransformPoint(planeGimoz[1]));
        Gizmos.DrawLine(this.transform.TransformPoint(planeGimoz[1]), this.transform.TransformPoint(planeGimoz[2]));
        Gizmos.DrawLine(this.transform.TransformPoint(planeGimoz[2]), this.transform.TransformPoint(planeGimoz[3]));
        Gizmos.DrawLine(this.transform.TransformPoint(planeGimoz[3]), this.transform.TransformPoint(planeGimoz[0]));
        Gizmos.DrawLine(this.transform.TransformPoint(planeGimoz[4]), this.transform.TransformPoint(planeGimoz[5]));
    }
#endif
}

