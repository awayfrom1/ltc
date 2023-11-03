using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateAreaLight : MonoBehaviour
{
    public List<GameObject> arealightPrefablist = new List<GameObject>();
    public GameObject arealightPrefab;
    
    public void GenerateAreaLight()
    {
        if (arealightPrefab == null)
        {
            return;
        }
        GameObject areaLightIns = Instantiate(arealightPrefab);
        areaLightIns.name = "ArealightLTC";
        areaLightIns.GetComponent<MeshRenderer>().enabled = false;
        arealightPrefablist.Add(arealightPrefab);
    }
    
    public void RemoveAreaLight()
    {
        if (arealightPrefablist.Count < 1)
        {
            return;
        }

        GameObject arealightPrefabDestory = arealightPrefablist[arealightPrefablist.Count - 1];
        DestroyImmediate(arealightPrefabDestory);
        arealightPrefablist.RemoveAt(arealightPrefablist.Count - 1);
    }
}
