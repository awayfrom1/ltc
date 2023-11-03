using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public partial class AreaLightLTC : MonoBehaviour
{
    const int kLUTResolution = 64;
    const int kLUTMatrixDim = 3;

    /// <summary>
    /// 贴图,就算有多个多光源，也只会全部加载一次贴图，所以设置为static
    /// </summary>
    static private Texture2D transformInvMatrixSpecularTexture;
    static private Texture2D transformInvMatrxDiffuseTexture;
    static private Texture2D ampDiffAmpSpecFresnelTexture;
    private int transformInvMatrixSpecularTextureID = Shader.PropertyToID("_transformInvMatrixSpecularTexture");
    private int transformInvMatrxDiffuseTextureID = Shader.PropertyToID("_transformInvMatrxDiffuseTexture");
    private int ampDiffAmpSpecFresnelTextureID = Shader.PropertyToID("_ampDiffAmpSpecFresnelTexture");

    /// <summary>
    /// 存三张图
    /// </summary>
    public enum LUTType
    {
        TransformInv_DisneyDiffuse,
        TransformInv_GGX,
        AmpDiffAmpSpecFresnel
    }

    public static Texture2D LoadLUT(LUTType type)
    {
        switch (type)
        {
            case LUTType.TransformInv_DisneyDiffuse: 
                return LoadLUT(s_LUTTransformInv_DisneyDiffuse);
            case LUTType.TransformInv_GGX: 
                return LoadLUT(s_LUTTransformInv_GGX);
            case LUTType.AmpDiffAmpSpecFresnel:
                return LoadLUT(s_LUTAmplitude_DisneyDiffuse, s_LUTAmplitude_GGX, s_LUTFresnel_GGX);
        }
        return null;
    }

    /// <summary>
    /// 加载lut
    /// </summary>
    /// <param name="LUTTransformInv"></param>
    /// <returns></returns>

    private static Texture2D LoadLUT(double[,] LUTTransformInv)
    {
        const int count = kLUTResolution * kLUTResolution;
        Color[] pixels = new Color[count];

        for (int i = 0; i < count; i++)
        {
            pixels[i] = new Color((float)LUTTransformInv[i, 0], 
                (float)LUTTransformInv[i, 2], 
                (float)LUTTransformInv[i, 4], 
                (float)LUTTransformInv[i, 6]);
        }
        return CreateLUT(TextureFormat.RGBAHalf, pixels);
    }

    static Texture2D LoadLUT(float[] LUTScalar0, float[] LUTScalar1, float[] LUTScalar2)
    {
        const int count = kLUTResolution * kLUTResolution; // pixels数量
        Color[] pixels = new Color[count];

        // amplitude
        for (int i = 0; i < count; i++)
        {
            pixels[i] = new Color(LUTScalar0[i], LUTScalar1[i], LUTScalar2[i], 0);
        }

        return CreateLUT(TextureFormat.RGBAHalf, pixels);
    }

    /// <summary>
    /// 创建Lut
    /// </summary>
    /// <param name="format"></param>
    /// <param name="pixels"></param>
    /// <returns></returns>

    static Texture2D CreateLUT(TextureFormat format, Color[] pixels)
    {
        Texture2D tex = new Texture2D(kLUTResolution, kLUTResolution, format, false, true);
        tex.hideFlags = HideFlags.HideAndDontSave;
        tex.wrapMode = TextureWrapMode.Clamp;
        tex.SetPixels(pixels);
        tex.Apply();
        return tex;
    }

    /// <summary>
    /// 设置存储矩阵的三张lut
    /// </summary>
    public void SetUpLut(CommandBuffer buffer)
    {
        if (transformInvMatrixSpecularTexture == null)
        {
            transformInvMatrixSpecularTexture = LoadLUT(LUTType.TransformInv_DisneyDiffuse);
        }
        if (transformInvMatrxDiffuseTexture == null)
        {
            transformInvMatrxDiffuseTexture = LoadLUT(LUTType.AmpDiffAmpSpecFresnel);
        }
        if (ampDiffAmpSpecFresnelTexture == null)
        {
            transformInvMatrxDiffuseTexture = LoadLUT(LUTType.TransformInv_GGX);
        }
        buffer.SetGlobalTexture(transformInvMatrixSpecularTextureID, transformInvMatrixSpecularTexture);
        buffer.SetGlobalTexture(transformInvMatrxDiffuseTextureID, transformInvMatrxDiffuseTexture);
        buffer.SetGlobalTexture(ampDiffAmpSpecFresnelTextureID, ampDiffAmpSpecFresnelTexture);
    }
}
