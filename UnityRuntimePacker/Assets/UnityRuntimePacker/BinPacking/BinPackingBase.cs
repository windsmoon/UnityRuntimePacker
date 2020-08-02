using System.Collections.Generic;
using UnityEngine;

namespace Windsmoon.UnityRuntimePacker.BinPacking
{
    public abstract class BinPackingBase
    {
        #region methods
        public abstract bool Pack(List<Item> itemList, out Vector2Int size);
        #endregion
    }
}