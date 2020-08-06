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
        protected List<SpriteInfo> spriteInfoList;
        protected List<Texture2D> texture2DList;
        protected List<Sprite> spriteList;
        protected Vector2Int resultSize;
        protected RenderTexture rt;
        protected FilterMode filterMode = FilterMode.Point;
        #endregion
        
        #region properties
        public BinPackingBase BinPacking
        {
            set => packerCore.PackStrategy = value;
        }

        public RenderTexture RT
        {
            get => rt;
        }
        #endregion
        
        #region constructors
        public BaseAgent()
        {
            packerCore = new PackerCore(new NextFitBinPacking());
        }
        #endregion
        
        #region methods
        public bool GenerateAtlas(List<Texture2D> texture2DList, List<Sprite> spriteList)
        {
            packerCore.Init();

            if (texture2DList == null && spriteList == null)
            {
                return false;
            }
            
            if (spriteInfoList == null)
            {
                spriteInfoList = new List<SpriteInfo>();
            }

            else
            {
                spriteInfoList.Clear();
            }

            this.texture2DList = texture2DList;
            this.spriteList = spriteList;

            if (texture2DList != null)
            {
                foreach (Texture2D texture2D in texture2DList)
                {
                    SpriteInfo spriteInfo = new SpriteInfo();
                    spriteInfo.ID = spriteInfoList.Count;
                    spriteInfo.type = 1;
                    spriteInfo.Size = new Vector2Int(texture2D.width, texture2D.height);
                    spriteInfoList.Add(spriteInfo);
                }
            }

            if (spriteList != null)
            {
                foreach (Sprite sprite in spriteList)
                {
                    SpriteInfo spriteInfo = new SpriteInfo();
                    spriteInfo.ID = spriteInfoList.Count;
                    spriteInfo.type = 1;
                    spriteInfo.Size = new Vector2Int((int)sprite.rect.width, (int)sprite.rect.height);
                    spriteInfoList.Add(spriteInfo);
                }
            }

            resultSize = new Vector2Int(1024, 1024);
            packerCore.UnInit();
            return packerCore.GenerateAtlas(spriteInfoList, ref resultSize);
        }

        public RenderTexture RenderToRT()
        {
            rt = packerCore.RenderToRT(spriteInfoList, resultSize, texture2DList, spriteList);
            rt.filterMode = filterMode;
            return rt;
        }
        
        public void ReplaceImage(List<Image> imageList)
        {
            rt.filterMode = filterMode;
        
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
                rawImage.uvRect = GetUV(i);
                rawImage.texture = rt;
                rawImage.color = color;
                rawImage.material = material;
                rawImage.raycastTarget = isRaycastTarget;
            }
        }
        
        public void ReleaseRT(bool justReleaseRTRef)
        {
            if (rt == null)
            {
                return;
            }

            if (justReleaseRTRef)
            {
                rt = null;
                return;
            }

            rt.Release();
        }

        public void Release(bool justReleaseRTRef)
        {
            ReleaseRT(justReleaseRTRef);
            packerCore.UnInit();
            packerCore = null;
            spriteInfoList = null;
            texture2DList = null;
            spriteList = null;
        }
        
        public void GetUVList(List<Rect> uvList)
        {
            uvList.Clear();
             
            foreach (SpriteInfo spriteInfo in spriteInfoList)
            {
                uvList.Add(ConverPosToUV(spriteInfo.Pos, spriteInfo.Size));
            }
        }
        
        public Rect GetUV(int id)
        {
            SpriteInfo spriteInfo = spriteInfoList[id];
            return ConverPosToUV(spriteInfo.Pos, spriteInfo.Size);
        }

        public void SetFilterMode(FilterMode filterMode)
        {
            this.filterMode = filterMode;

            if (rt != null)
            {
                rt.filterMode = filterMode;
            }
        }

        protected Vector2 ConvertPosToUV(Vector2 pos)
        {
             return new Vector2(pos.x / rt.width, pos.y / rt.height);
        }

        protected Rect ConverPosToUV(Vector2 pos, Vector2 size)
        {
             return new Rect(pos.x / rt.width, pos.y / rt.height, size.x / rt.width, size.y / rt.height);
        }
        #endregion
    }
}
