// using System;
// using System.Collections.Generic;
// using UnityEngine;
// using Windsmoon.UnityRuntimePacker.BinPacking;
//
// namespace Windsmoon.UnityRuntimePacker
// {
//     public class Atlas : IDisposable
//     {
//         #region fields
//         private RenderTexture rt;
//         private List<Item> itemList;
//         #endregion
//
//         #region properties
//         public RenderTexture RT
//         {
//             get { return rt; }
//         }
//
//         public int SpriteCount
//         {
//             get => itemList.Count;
//         }
//         #endregion
//         
//         #region constructors
//         public Atlas(RenderTexture rt, List<Item> itemList)
//         {
//             this.rt = rt;
//             this.itemList = itemList;
//         }
//         #endregion
//
//         #region interfaces
//         public void Dispose()
//         {
//             rt.Release();
//         }
//         #endregion
//
//         #region methods
//         public void GetUVList(List<Rect> uvList)
//         {
//             uvList.Clear();
//             
//             foreach (Item item in itemList)
//             {
//                 uvList.Add(ConverPosToUV(item.Pos, item.Size));
//             }
//         }
//
//         public Rect GetUV(int id)
//         {
//             Item item = itemList[id];
//             return ConverPosToUV(item.Pos, item.Size);
//         }
//
//         private Vector2 ConvertPosToUV(Vector2 pos)
//         {
//             return new Vector2(pos.x / rt.width, pos.y / rt.height);
//         }
//
//         private Rect ConverPosToUV(Vector2 pos, Vector2 size)
//         {
//             return new Rect(pos.x / rt.width, pos.y / rt.height, size.x / rt.width, size.y / rt.height);
//         }
//         #endregion
//     }
// }