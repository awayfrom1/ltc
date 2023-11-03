using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]

public partial class AreaLightLTC : MonoBehaviour
{
    [Header("LightAttribute:")]
    public Color lightColor = Color.white;
    [MinValue(0.0f)]
    public float lightIntensity = 1;
    public Vector3 lightSize = Vector3.one;
    [Range(0.0f, 1.0f)] public float lightRoughness = 0.5f;

    private int AreaLightEmissionID = Shader.PropertyToID("_AreaLightEmission");

    private void Awake()
    {
        
    }

    private void OnEnable()
    {
        AreaLightLTCManager.Instance.Add(this);
    }

    private void Update()
    {
        
    }

    private void OnDisable()
    {
        AreaLightLTCManager.Instance.Remove(this);
    }

    private void OnDestroy()
    {

    }
    

    private Bounds GetFrustumbound()
    {
        return new Bounds();
    }

#if UNITY_EDITOR
    //绘制Gizmo
    private void OnDrawGizmos()
    {
        Gizmos.DrawLine(this.transform.TransformPoint(new Vector3(this.lightSize.x * AreaLightLTCManager.Instance.offsets[0, 0],
                       this.lightSize.y * AreaLightLTCManager.Instance.offsets[0, 1], 0.01f) * 0.5f), 
                       this.transform.TransformPoint(new Vector3(this.lightSize.x * AreaLightLTCManager.Instance.offsets[1, 0],
                       this.lightSize.y * AreaLightLTCManager.Instance.offsets[1, 1], 0.01f) * 0.5f)
                       );
        Gizmos.DrawLine(this.transform.TransformPoint(new Vector3(this.lightSize.x * AreaLightLTCManager.Instance.offsets[1, 0],
                      this.lightSize.y * AreaLightLTCManager.Instance.offsets[1, 1], 0.01f) * 0.5f),
                      this.transform.TransformPoint(new Vector3(this.lightSize.x * AreaLightLTCManager.Instance.offsets[2, 0],
                      this.lightSize.y * AreaLightLTCManager.Instance.offsets[2, 1], 0.01f) * 0.5f)
                      );
        Gizmos.DrawLine(this.transform.TransformPoint(new Vector3(this.lightSize.x * AreaLightLTCManager.Instance.offsets[2, 0],
                      this.lightSize.y * AreaLightLTCManager.Instance.offsets[2, 1], 0.01f) * 0.5f),
                      this.transform.TransformPoint(new Vector3(this.lightSize.x * AreaLightLTCManager.Instance.offsets[3, 0],
                      this.lightSize.y * AreaLightLTCManager.Instance.offsets[3, 1], 0.01f) * 0.5f)
                      );
        Gizmos.DrawLine(this.transform.TransformPoint(new Vector3(this.lightSize.x * AreaLightLTCManager.Instance.offsets[3, 0],
                      this.lightSize.y * AreaLightLTCManager.Instance.offsets[3, 1], 0.01f) * 0.5f),
                      this.transform.TransformPoint(new Vector3(this.lightSize.x * AreaLightLTCManager.Instance.offsets[0, 0],
                      this.lightSize.y * AreaLightLTCManager.Instance.offsets[0, 1], 0.01f) * 0.5f)
                      );
    }
#endif
}
