using System.Collections.Generic;
using UnityEngine;

namespace Windsmoon.UnityRuntimePacker.BinPacking
{
    public class NextFitBinPacking : BinPackingBase
    {
        #region Methods
        public override bool Pack(ref List<Item> itemList, out Vector2Int size)
        {
            if (itemList == null || itemList.Count == 0)
            {
                size = Vector2Int.zero;
                return false;
            }
            
            size = new Vector2Int(2048, 2048);
            int currentBinIndex = 0;
            int leftHeight = 2047;
            int currentX = 1;
            int currentMaxX = 0;

            for (int i = 0; i < itemList.Count; ++i)
            {
                Item item = itemList[i];
                Vector2Int itemSize = item.Size;

                if (leftHeight > itemSize.y)
                {
                    if (itemSize.x > currentMaxX)
                    {
                        currentMaxX = itemSize.x;
                    }
                    
                    item.Pos = new Vector2Int(currentX, leftHeight);
                    itemList[i] = item;
                    leftHeight -= itemSize.y - 1;
                }

                else
                {
                    leftHeight = 2047;
                    currentX += currentMaxX + 1;
                    currentMaxX = itemSize.x; // do not consider the situation that item height is bigger than bin height;
                    item.Pos = new Vector2Int(currentX, leftHeight);
                    itemList[i] = item;
                    leftHeight -= itemSize.y - 1;
                }
            }
            
            return true;
        }
        #endregion
    }
}