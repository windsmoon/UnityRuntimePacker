using UnityEngine;

namespace Windsmoon.UnityRuntimePacker
{
    public class Atlas
    {
        #region fields
        private RenderTexture rt;
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