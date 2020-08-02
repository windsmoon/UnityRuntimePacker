using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Windsmoon.UnityRuntimePacker;
using Windsmoon.UnityRuntimePacker.BinPacking;

public class Test : MonoBehaviour
{
    #region fields
    [SerializeField]
    private Texture2D testImage;
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
        List<Texture2D> texture2DList = new List<Texture2D>();
        
        for (int i = 0; i < 10; ++i)
        {
            texture2DList.Add(testImage);
        }
        
        PackerCore.PackStrategy = new NextFitBinPacking();
        PackerCore.Init();
        List<Item> itemList = new List<Item>(texture2DList.Count);
        PackerCore.GenerateAtlas(itemList, texture2DList);
    }
    #endregion
}
