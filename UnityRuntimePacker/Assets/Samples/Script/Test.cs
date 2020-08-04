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
    private List<Sprite> spriteList;
    private BaseAgent baseAgent;
    #endregion

    #region unity methods
    // private void OnRenderImage(RenderTexture src, RenderTexture dest)
    // {
    //     Graphics.Blit(rt, dest);
    // }
    #endregion
    
    #region methods
    [ContextMenu("Test")]
    private void TestPacker()
    {
        if (baseAgent != null)
        {
            this.baseAgent.Release();
        }
        
        List<Image> imageList = new List<Image>();
        canvas.GetComponentsInChildren<Image>(imageList);
        spriteList = new List<Sprite>();

        foreach (Image image in imageList)
        {
            spriteList.Add(image.sprite);
        }
        
        baseAgent = new BaseAgent();
        baseAgent.SetFilterMode(FilterMode.Point);
        baseAgent.GenerateAtlasAndReplace(imageList);
        rt = baseAgent.RT;
    }
    
    [ContextMenu("Rest")]
    private void Rest()
    {
        List<RawImage> rawImageList = new List<RawImage>();
        canvas.GetComponentsInChildren<RawImage>(rawImageList);

        int i = 0;
        
        foreach (RawImage rawImage in rawImageList)
        {
            GameObject go = rawImage.gameObject;
            UnityEngine.Object.DestroyImmediate(rawImage);
            Image image = go.AddComponent<Image>();
            image.sprite = spriteList[i];
            ++i;
        }
    }
    #endregion
}
