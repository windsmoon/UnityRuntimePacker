using System.Collections.Generic;
using UnityEngine;

namespace Windsmoon.UnityRuntimePacker
{
    public class Atlas
    {
        #region fields
        private RenderTexture rt;
        private List<SpriteInfo> spriteInfoList;
        #endregion

        #region properties
        public RenderTexture Rt
        {
            get { return rt; }
        }
        #endregion
        
        #region constructors
        public Atlas(RenderTexture rt)
        {
            this.rt = rt;
        }
        #endregion
    }
}