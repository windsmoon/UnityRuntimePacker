using System.Collections.Generic;
using UnityEngine;

namespace Windsmoon.UnityRuntimePacker.BinPacking
{
    public class NextFitBinPacking : BinPackingBase
    {
        #region Methods
        public override bool Pack(List<Item> itemList, out Vector2Int size)
        {
            if (itemList == null || itemList.Count == 0)
            {
                size = Vector2Int.zero;
                return false;
            }
            
            // todo
            size = new Vector2Int(2048, 2048);
            int interval = 10;
            int currentBinIndex = 0;
            int leftHeight = 2048 - interval;
            int currentX = interval;
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
                    
                    item.Pos = new Vector2Int(currentX, size.y - leftHeight);
                    itemList[i] = item;
                    leftHeight = leftHeight - itemSize.y - interval;
                }

                else
                {
                    leftHeight = 2048 - interval;
                    currentX += currentMaxX + interval;
                    currentMaxX = itemSize.x; // do not consider the situation that item height is bigger than bin height;
                    item.Pos = new Vector2Int(currentX, size.y - leftHeight);
                    itemList[i] = item;
                    leftHeight = leftHeight - itemSize.y - interval;;
                }
            }
            
            return true;
        }
        #endregion
    }
}