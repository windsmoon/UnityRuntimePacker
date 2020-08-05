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
        // private GameObject cameraGO;
        // private Camera camera;
        // private Material templateMaterial;
        // private GameObject templateGO;
        // private bool useMiniMap;
        private Vector2Int size = new Vector2Int(2048, 2048);
        private static int mainTexPropertyID = Shader.PropertyToID("_MainTex");
        #endregion

        #region properties
        public BinPackingBase PackStrategy
        {
            get { return packStrategy; }
            set { packStrategy = value; }
        }

        public Vector2Int Size
        {
            get => size;
            set => size = value;
        }

        #endregion

        #region constructors
        public PackerCore(BinPackingBase packStrategy, bool useMiniMap = false)
        {
            this.packStrategy = packStrategy;
            this.useMiniMap = useMiniMap;
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

        public Atlas GenerateAtlas(List<Texture2D> texture2DList)
        {
            return GenerateAtlas(new List<Item>(texture2DList.Count), texture2DList);
        }
        
        public Atlas GenerateAtlas(List<Item> itemList, List<Texture2D> texture2DList)
        {
            itemList.Clear();
            
            for (int i = 0; i < texture2DList.Count; ++i)
            {
                Texture2D texture2D = texture2DList[i];
                Item item = new Item();
                item.ID = i;
                item.Size = new Vector2Int(texture2D.width, texture2D.height);
                itemList.Add(item); 
            }

            if (!packStrategy.Pack(itemList, ref size))
            {
                return null;
            }
            
            templateGO.SetActive(true);
            List<GameObject> goList = new List<GameObject>(itemList.Count); // todo : can be cached
            List<Material> materialList = new List<Material>(itemList.Count); // todo : cam be cached
            RenderTexture rt = new RenderTexture(size.x, size.y, 0, RenderTextureFormat.ARGB32);
            rt.useMipMap = useMiniMap;
            SetCamera(rt);
            
            for (int i = 0; i < itemList.Count; ++i)
            {
                Item item = itemList[i];
                Texture2D texture2D = texture2DList[item.ID];
                Material material = UnityEngine.Object.Instantiate(templateMaterial);
                material.SetTexture(mainTexPropertyID, texture2D);
                GameObject go = UnityEngine.Object.Instantiate(templateGO);
                go.GetComponent<MeshRenderer>().sharedMaterial = material;
                SetGO(item, go);
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
            Atlas atlas = new Atlas(rt, itemList);
            return atlas;
        }
        
        private void SetCamera(RenderTexture rt)
        {
            camera.targetTexture = rt;
            float halfCameraHeight = rt.height / 100f / 2f;
            float halfCameraWidth = ((float) rt.width / rt.height) * halfCameraHeight;
            camera.orthographicSize = halfCameraHeight;
            camera.transform.Translate(halfCameraWidth, halfCameraHeight, 0);
        }

        private static void SetGO(Item item, GameObject go)
        {
            Transform goTransform = go.transform;
            Vector2 goPos = PixelToCoord(item.Pos);
            Vector2 goSize = PixelToCoord(item.Size);
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
