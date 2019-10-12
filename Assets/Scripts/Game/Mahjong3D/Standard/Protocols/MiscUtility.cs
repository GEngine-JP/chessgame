using UnityEngine;
using System;

namespace Assets.Scripts.Game.Mahjong3D.Standard
{
    /// <summary>
    /// 默认参数
    /// </summary>
    public class MiscUtility
    {
        //默认值 
        public const int DefValue = -1;
        public const int DefInt = Int32.MaxValue;
        public const string DefName = "default";
        public static readonly Vector3 MahjongSize = new Vector3(0.3f, 0.4f, 0.22f); //麻将默认长宽高
    }

    public struct FindGangData
    {
        public int type;
        public int ttype;
        public int[] cards;
    }
}