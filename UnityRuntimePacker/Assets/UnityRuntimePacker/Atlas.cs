using System.Collections.Generic;
using UnityEngine;
using Windsmoon.UnityRuntimePacker.BinPacking;

namespace Windsmoon.UnityRuntimePacker
{
    public class Atlas
    {
        #region fields
        private RenderTexture rt;
        private List<Item> itemList;
        #endregion

        #region properties
        public RenderTexture Rt
        {
            get { return rt; }
        }

        public Item this[int index]
        {
            get
            {
                return itemList[index];
            }
        }
        #endregion
        
        #region constructors
        public Atlas(RenderTexture rt, List<Item> itemList)
        {
            this.rt = rt;
            this.itemList = itemList;
        }

        public void GetUVList(List<Vector2> uvList)
        {
            uvList.Clear();
            
            foreach (Item item in itemList)
            {
                uvList.Add(ConvertPosToUV(item.Pos));
            }
        }

        private Vector2 ConvertPosToUV(Vector2 pos)
        {
            return new Vector2(pos.x / rt.width, pos.y / rt.height);
        }
        #endregion
    }
}