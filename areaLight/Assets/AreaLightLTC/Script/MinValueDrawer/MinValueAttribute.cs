using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinValueAttribute : PropertyAttribute
{
    public float min;

    public MinValueAttribute(float min)
    {
        this.min = min;
    }
        
}
