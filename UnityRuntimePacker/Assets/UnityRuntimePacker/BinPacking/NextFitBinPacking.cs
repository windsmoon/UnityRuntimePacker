using System.Collections.Generic;
using UnityEngine;

namespace Windsmoon.UnityRuntimePacker.BinPacking
{
    public class NextFitBinPacking : BinPackingBase
    {
        #region Methods
        public override bool Pack(List<SpriteInfo> spriteInfoList, ref Vector2Int size)
        {
            if (spriteInfoList == null || spriteInfoList.Count == 0)
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

            for (int i = 0; i < spriteInfoList.Count; ++i)
            {
                SpriteInfo spriteInfo = spriteInfoList[i];
                Vector2Int spriteSize = spriteInfo.Size;

                if (leftHeight > spriteSize.y)
                {
                    if (spriteSize.x > currentMaxX)
                    {
                        currentMaxX = spriteSize.x;
                    }
                    
                    spriteInfo.Pos = new Vector2Int(currentX, size.y - leftHeight);
                    spriteInfoList[i] = spriteInfo;
                    leftHeight = leftHeight - spriteSize.y - interval;

                    if (leftHeight < 0)
                    {
                        return false;
                    }

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
                    currentMaxX = spriteSize.x; // do not consider the situation that item height is bigger than bin height;
                    spriteInfo.Pos = new Vector2Int(currentX, size.y - leftHeight);
                    spriteInfoList[i] = spriteInfo;
                    leftHeight = leftHeight - spriteSize.y - interval;
                    
                    if (leftHeight < 0)
                    {
                        return false;
                    }
                    
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