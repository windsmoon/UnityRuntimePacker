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
        protected FilterMode filterMode = FilterMode.Point;
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
            packerCore.Size = new Vector2Int(4096, 512);
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
            atlas.RT.filterMode = filterMode;

            if (atlas == null)
            {
                return;
            }

            for (int i = 0; i < imageList.Count; ++i)
            {
                Image image = imageList[i];
                Color color = image.color;
                Material material = image.material;
                bool isRaycastTarget = image.raycastTarget;                
                GameObject go = image.gameObject;
                UnityEngine.Object.DestroyImmediate(image);
                // todo
                RawImage rawImage = go.AddComponent<RawImage>();
                rawImage.uvRect = atlas.GetUV(i);
                rawImage.texture = atlas.RT;
                rawImage.color = color;
                rawImage.material = material;
                rawImage.raycastTarget = isRaycastTarget;
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

        public void SetFilterMode(FilterMode filterMode)
        {
            this.filterMode = filterMode;

            if (atlas != null)
            {
                atlas.RT.filterMode = filterMode;
            }
        }

        public void Release()
        {
            if (atlas != null)
            {
                atlas.Dispose();
            }
            
            packerCore.UnInit();
        }
        #endregion
    }
}
