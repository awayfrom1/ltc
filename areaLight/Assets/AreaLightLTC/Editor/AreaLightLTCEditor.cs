using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class AreaLightLTCEditor : Editor
{
    public static string AreaLightPath = "Assets/AreaLightLTC/Prefab/areaLightLTC.prefab";

    [MenuItem("GameObject/Light/AreaLightLTC")]
    public static void GenerateAreaLight()
    {
        GameObject arealight = AssetDatabase.LoadAssetAtPath(AreaLightPath, typeof(GameObject)) as GameObject;
        if (arealight == null)
        {
            Debug.LogError("当前areaLight不存在，请检查路径");
            return;
        }
        GameObject areaLightIns = Instantiate(arealight);
        areaLightIns.name = "ArealightLTC";
        areaLightIns.GetComponent<MeshRenderer>().enabled = false;
    }
}