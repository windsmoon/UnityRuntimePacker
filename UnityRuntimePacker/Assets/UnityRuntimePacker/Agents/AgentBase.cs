using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Windsmoon.UnityRuntimePacker.BinPacking;

namespace Windsmoon.UnityRuntimePacker.Agents
{
    public class BaseAgent
    {
        #region fields
        protected PackerCore packerCore;
        protected List<Item> itemList;
        protected List<Texture2D> texture2DList;
        protected Atlas atlas;
        #endregion
        
        #region properties
        public BinPackingBase BinPacking
        {
            set => packerCore.PackStrategy = value;
        }

        public RenderTexture RT
        {
            get => atlas.RT;
        }

        public int SpriteCount
        {
            get => atlas.SpriteCount;
        }
        #endregion
        
        #region constructors
        public BaseAgent()
        {
            packerCore = new PackerCore(new NextFitBinPacking());
            packerCore.Init();
        }
        #endregion
        
        #region methods
        public virtual void GenerateAtlasAndReplace(List<Image> imageList)
        {
            if (texture2DList == null)
            {
                texture2DList = new List<Texture2D>();
            }

            else
            {
                texture2DList.Clear();
            }

            foreach (Image image in imageList)
            {
                texture2DList.Add(image.sprite.texture);
            }
            
            atlas = GenerateAtlas(texture2DList);
            atlas.RT.filterMode = FilterMode.Bilinear;

            if (atlas == null)
            {
                return;
            }

            for (int i = 0; i < imageList.Count; ++i)
            {
                Image image = imageList[i];
                GameObject go = image.gameObject;
                UnityEngine.Object.DestroyImmediate(image);
                // todo
                RawImage rawImage = go.AddComponent<RawImage>();
                rawImage.uvRect = atlas.GetUV(i);
                rawImage.texture = atlas.RT;
                // Texture2D t;
            }
        }
        
        public virtual Atlas GenerateAtlas(List<Texture2D> texture2DList)
        {
            
            if (itemList == null)
            {
                itemList = new List<Item>(texture2DList.Count);
            }
            
            return packerCore.GenerateAtlas(itemList, texture2DList);
        }

        public RenderTexture GetRT()
        {
            return atlas.RT;
        }

        public void GetUVList(List<Rect> uvList)
        {
            atlas.GetUVList(uvList);
        }
        
        public Rect GetUV(int id)
        {
            return atlas.GetUV(id);
        }

        public void Release()
        {
            atlas.Dispose();
            packerCore.UnInit();
        }
        #endregion
    }
}
