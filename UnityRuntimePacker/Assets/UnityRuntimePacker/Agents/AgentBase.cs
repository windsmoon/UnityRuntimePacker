using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Windsmoon.UnityRuntimePacker.Agents
{
    public class AgentBase
    {
        #region methods
        public virtual void Init()
        {
            PackerCore.Init();
        }

        public virtual Atlas GenerateAtlas(List<Texture2D> texture2DList)
        {
            return null;
        }

        public virtual void UnInit()
        {
            PackerCore.UnInit();
        }
        #endregion
    }
}
