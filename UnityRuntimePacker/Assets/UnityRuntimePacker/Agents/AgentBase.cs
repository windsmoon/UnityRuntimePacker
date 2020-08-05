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
        // protected List<Item> itemList;
        protected List<SpriteInfo> spriteInfoList;
        protected List<Texture2D> texture2DList;
        protected List<Sprite> spriteList;
        // protected Atlas atlas;
        protected Vector2Int resultSize;
        protected RenderTexture rt;
        protected FilterMode filterMode = FilterMode.Point;
        protected bool useMiniMap;
        protected GameObject cameraGO;
        protected Camera camera;
        protected Material templateMaterial;
        protected GameObject templateGO;
        protected static int mainTexPropertyID = Shader.PropertyToID("_MainTex");
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

        public int SpriteCount
        {
            get => spriteInfoList.Count;
        }
        #endregion
        
        #region constructors
        public BaseAgent()
        {
            packerCore = new PackerCore(new NextFitBinPacking());
            packerCore.Init();
            packerCore.Size = new Vector2Int(2048, 2048);
        }
        #endregion
        
        #region methods
        // public virtual bool GenerateAtlasAndReplace(List<Image> imageList)
        // {
        //     if (texture2DList == null)
        //     {
        //         texture2DList = new List<Texture2D>();
        //     }
        //
        //     else
        //     {
        //         texture2DList.Clear();
        //     }
        //
        //     foreach (Image image in imageList)
        //     {
        //         texture2DList.Add(image.sprite.texture);
        //     }
        //
        //     if (!GenerateAtlas(texture2DList))
        //     {
        //         return false;
        //     }
        //     
        //     atlas.RT.filterMode = filterMode;
        //
        //     for (int i = 0; i < imageList.Count; ++i)
        //     {
        //         Image image = imageList[i];
        //         Color color = image.color;
        //         Material material = image.material;
        //         bool isRaycastTarget = image.raycastTarget;                
        //         GameObject go = image.gameObject;
        //         UnityEngine.Object.DestroyImmediate(image);
        //         // todo
        //         RawImage rawImage = go.AddComponent<RawImage>();
        //         rawImage.uvRect = atlas.GetUV(i);
        //         rawImage.texture = atlas.RT;
        //         rawImage.color = color;
        //         rawImage.material = material;
        //         rawImage.raycastTarget = isRaycastTarget;
        //         // Texture2D t;
        //     }
        //
        //     return true;
        // }
        
        public bool GenerateAtlas(List<Texture2D> texture2DList, List<Sprite> spriteList)
        {
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

            foreach (Texture2D texture2D in texture2DList)
            {
                SpriteInfo spriteInfo = new SpriteInfo();
                spriteInfo.type = 1;
                spriteInfo.Size = new Vector2Int(texture2D.width, texture2D.height);
                spriteInfoList.Add(spriteInfo);
            }

            resultSize = new Vector2Int();
            return packerCore.GenerateAtlas(spriteInfoList, ref resultSize);
        }

        public RenderTexture RenderToRT()
        {
            if (!cameraGO)
            {
                UnityEngine.Object.Destroy(cameraGO);
                cameraGO = new GameObject();
                camera = cameraGO.AddComponent<Camera>();
                camera.enabled = false;
                camera.orthographic = true;
                camera.backgroundColor = new Color(0, 0, 0, 0);
                camera.clearFlags = CameraClearFlags.SolidColor;
            }

            if (!templateMaterial)
            {
                templateMaterial = new Material(Shader.Find("UI/Default"));
            }

            if (!templateGO)
            {
                templateGO = GameObject.CreatePrimitive(PrimitiveType.Quad);
                templateGO.transform.localScale = new Vector3(0, 0, 0);
            }
            
            templateGO.SetActive(true);
            List<GameObject> goList = new List<GameObject>(spriteInfoList.Count); // todo : can be cached
            List<Material> materialList = new List<Material>(spriteInfoList.Count); // todo : cam be cached
            rt = new RenderTexture(resultSize.x, resultSize.y, 0, RenderTextureFormat.ARGB32);
            rt.useMipMap = useMiniMap;
            SetCamera(rt);
            
            for (int i = 0; i < spriteInfoList.Count; ++i)
            {
                SpriteInfo spriteInfo = spriteInfoList[i];
                Texture2D texture2D = texture2DList[spriteInfo.ID];
                Material material = UnityEngine.Object.Instantiate(templateMaterial);
                material.SetTexture(mainTexPropertyID, texture2D);
                GameObject go = UnityEngine.Object.Instantiate(templateGO);
                go.GetComponent<MeshRenderer>().sharedMaterial = material;
                SetGO(spriteInfo, go);
                materialList.Add(material);
                goList.Add(go);
            }
            
            camera.Render();
            camera.targetTexture = null;

            foreach (Material material in materialList)
            {
                UnityEngine.Object.Destroy(material);
            }
            
            foreach (GameObject gameObject in goList)
            {
                UnityEngine.Object.Destroy(gameObject);
            }
            
            templateGO.SetActive(false);
            return rt;
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

        public void ReleaseRT()
        {
            rt.Release();
            rt = null;
        }
        
        // todo release packcore
        
        protected void SetCamera(RenderTexture rt)
        {
            camera.targetTexture = rt;
            float halfCameraHeight = rt.height / 100f / 2f;
            float halfCameraWidth = ((float) rt.width / rt.height) * halfCameraHeight;
            camera.orthographicSize = halfCameraHeight;
            camera.transform.Translate(halfCameraWidth, halfCameraHeight, 0);
        }

        protected static void SetGO(SpriteInfo spriteInfo, GameObject go)
        {
            Transform goTransform = go.transform;
            Vector2 goPos = PixelToCoord(spriteInfo.Pos);
            Vector2 goSize = PixelToCoord(spriteInfo.Size);
            goTransform.localScale = new Vector3(goSize.x, goSize.y, 1);
            goTransform.position = new Vector3(goPos.x + goSize.x / 2, goPos.y + goSize.y / 2, 1);
        }

        protected static float PixelToCoord(float pixel)
        {
            return pixel / 100;
        }

        protected static Vector2 PixelToCoord(Vector2 pixel)
        {
            return pixel / 100;
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
