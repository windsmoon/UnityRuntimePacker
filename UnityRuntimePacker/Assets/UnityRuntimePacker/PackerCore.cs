using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using Windsmoon.UnityRuntimePacker.BinPacking;

//http://davikingcode.com/blog/unity-generate-spritesheets-at-runtime/
//https://blog.uwa4d.com/archives/Severe_MOBA.html?tdsourcetag=s_pcqq_aiomsg

namespace Windsmoon.UnityRuntimePacker
{
    public class PackerCore
    {
        #region fields
        private BinPackingBase packStrategy;
        private GameObject cameraGO;
        private Camera camera;
        private Material templateMaterial;
        private GameObject templateGO;
        private bool useMiniMap;
        private static int mainTexPropertyID = Shader.PropertyToID("_MainTex");
        #endregion

        #region properties
        public BinPackingBase PackStrategy
        {
            get { return packStrategy; }
            set { packStrategy = value; }
        }
        #endregion

        #region constructors
        public PackerCore(BinPackingBase packStrategy, bool useMiniMap = false)
        {
            this.packStrategy = packStrategy;
            // this.useMiniMap = useMiniMap;
        }
        #endregion
        
        #region methods
        public void Init()
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
        }

        public void UnInit()
        {
            if (cameraGO)
            {
                UnityEngine.Object.Destroy(cameraGO);
            }

            if (templateMaterial)
            {
                UnityEngine.Object.Destroy(templateMaterial);
            }
            
            if (templateGO)
            {
                UnityEngine.Object.Destroy(templateGO);
            }
        }

        public bool GenerateAtlas(List<SpriteInfo> spriteInfoList, ref Vector2Int size)
        {
            return packStrategy.Pack(spriteInfoList, ref size);
        }

        public RenderTexture RenderToRT(List<SpriteInfo> spriteInfoList, Vector2Int size, List<Texture2D> texture2DList, List<Sprite> spriteList)
        {
            templateGO.SetActive(true);
            List<GameObject> goList = new List<GameObject>(spriteInfoList.Count); // todo : can be cached
            List<Material> materialList = new List<Material>(spriteInfoList.Count); // todo : cam be cached
            RenderTexture rt = new RenderTexture(size.x, size.y, 0, RenderTextureFormat.ARGB32);
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
        
        // todo
        public Texture GenerateTexture(List<SpriteInfo> spriteInfoList, Vector2Int size, List<Texture2D> texture2DList, List<Sprite> spriteList)
        {
            return null;
        }
        
        private void SetCamera(RenderTexture rt)
        {
            camera.targetTexture = rt;
            float halfCameraHeight = rt.height / 100f / 2f;
            float halfCameraWidth = ((float) rt.width / rt.height) * halfCameraHeight;
            camera.orthographicSize = halfCameraHeight;
            camera.transform.Translate(halfCameraWidth, halfCameraHeight, 0);
        }

        private static void SetGO(SpriteInfo spriteInfo, GameObject go)
        {
            Transform goTransform = go.transform;
            Vector2 goPos = PixelToCoord(spriteInfo.Pos);
            Vector2 goSize = PixelToCoord(spriteInfo.Size);
            goTransform.localScale = new Vector3(goSize.x, goSize.y, 1);
            goTransform.position = new Vector3(goPos.x + goSize.x / 2, goPos.y + goSize.y / 2, 1);
        }

        private static float PixelToCoord(float pixel)
        {
            return pixel / 100;
        }

        private static Vector2 PixelToCoord(Vector2 pixel)
        {
            return pixel / 100;
        }
        #endregion
    }
}
