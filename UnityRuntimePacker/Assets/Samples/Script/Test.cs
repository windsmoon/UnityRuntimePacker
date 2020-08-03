using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using Windsmoon.UnityRuntimePacker;
using Windsmoon.UnityRuntimePacker.Agents;
using Windsmoon.UnityRuntimePacker.BinPacking;

public class Test : MonoBehaviour
{
    #region fields
    [SerializeField]
    private Texture2D testImage;
    [SerializeField]
    private RenderTexture rt;
    [SerializeField]
    private GameObject canvas;
    #endregion

    #region unity methods
    private void OnRenderImage(RenderTexture src, RenderTexture dest)
    {
        Graphics.Blit(rt, dest);
    }
    #endregion
    
    #region methods
    [ContextMenu("Test UI Default")]
    private void TestUIDefault()
    {
        GameObject go = GameObject.CreatePrimitive(PrimitiveType.Quad);
        Material material = new Material(Shader.Find("UI/Default"));
        material.SetTexture("_MainTex", testImage);
        go.GetComponent<MeshRenderer>().sharedMaterial = material;
    }

    [ContextMenu("Test")]
    private void TestPacker()
    {
        List<Image> imageList = new List<Image>();
        canvas.GetComponentsInChildren<Image>(imageList);
        BaseAgent baseAgent = new BaseAgent();
        baseAgent.GenerateAtlasAndReplace(imageList);
        // PackerCore.PackStrategy = new NextFitBinPacking();
        // PackerCore.Init();
        // List<Item> itemList = new List<Item>(texture2DList.Count);
        // Atlas atlas = PackerCore.GenerateAtlas(itemList, texture2DList);
        // PackerCore.UnInit();
        // rt = atlas.Rt;
        // Graphics.Blit(atlas.Rt, (RenderTexture)null);
    }
    #endregion
}
