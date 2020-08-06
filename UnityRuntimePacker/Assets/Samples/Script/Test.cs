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

    private void Awake()
    {
        // TestPacker();
    }

    #endregion
    
    #region methods
    [ContextMenu("Test")]
    private void TestPacker()
    {
        List<Image> imageList = new List<Image>();
        canvas.GetComponentsInChildren<Image>(imageList);
        spriteList = new List<Sprite>(imageList.Count);
        List<Texture2D> texture2DList = new List<Texture2D>(imageList.Count);

        foreach (Image image in imageList)
        {
            spriteList.Add(image.sprite);
            texture2DList.Add(image.sprite.texture);
        }

        baseAgent = new BaseAgent();
        baseAgent.SetFilterMode(FilterMode.Point);

        if (baseAgent.GenerateAtlas(texture2DList, null))
        {
            rt = baseAgent.RenderToRT();
        }

        baseAgent.ReplaceImage(imageList);
        baseAgent.Release(true);
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
