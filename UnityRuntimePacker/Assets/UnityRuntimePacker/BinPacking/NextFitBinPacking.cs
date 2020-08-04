using System.Collections.Generic;
using UnityEngine;

namespace Windsmoon.UnityRuntimePacker.BinPacking
{
    public class NextFitBinPacking : BinPackingBase
    {
        #region Methods
        public override bool Pack(List<Item> itemList, ref Vector2Int size)
        {
            if (itemList == null || itemList.Count == 0)
            {
                size = Vector2Int.zero;
                return false;
            }
            
            // todo
            int interval = 10;
            int currentBinIndex = 0;
            int leftHeight = size.y - interval;
            int currentX = interval;
            int currentMaxX = 0;
            int minLeftHeight = size.y;
            int leftWdith = size.x - interval;

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

                    if (leftHeight < minLeftHeight)
                    {
                        minLeftHeight = leftHeight;  
                    }
                }

                else
                {
                    leftWdith = leftWdith - currentMaxX - interval; // another column
                    leftHeight = size.y - interval;
                    currentX += currentMaxX + interval;
                    currentMaxX = itemSize.x; // do not consider the situation that item height is bigger than bin height;
                    item.Pos = new Vector2Int(currentX, size.y - leftHeight);
                    itemList[i] = item;
                    leftHeight = leftHeight - itemSize.y - interval;;
                    
                    if (leftHeight < minLeftHeight)
                    {
                        minLeftHeight = leftHeight;
                    }
                }
            }

            leftWdith = leftWdith - currentMaxX - interval;
            size = new Vector2Int(size.x - leftWdith, size.y - minLeftHeight);
            return true;
        }
        #endregion
    }
}