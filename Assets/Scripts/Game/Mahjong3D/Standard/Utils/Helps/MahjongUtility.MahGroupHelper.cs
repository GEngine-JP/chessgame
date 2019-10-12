using System.Collections.Generic;
using UnityEngine;
using System;

namespace Assets.Scripts.Game.Mahjong3D.Standard
{
    public partial class MahjongUtility
    {
        /// <summary>
        /// 排序麻将
        /// </summary>  
        public static void SortMahjong(List<int> cards)
        {
            SortMahjong(cards, GameCenter.DataCenter.Game.LaiziCard);
        }

        public static void SortMahjong(List<int> cards, int laizi)
        {
            cards.Sort((a1, a2) =>
            {
                if (a1 == laizi && a2 != laizi) return -1;
                if (a1 != laizi && a2 == laizi) return 1;
                if (a1 < a2) return -1;
                if (a1 > a2) return 1;
                return 0;
            });
        }

        /// <summary>
        /// 麻将排序方法
        /// </summary>         
        public static void SortMahjong(List<MahjongContainer> cards, Func<MahjongContainer, MahjongContainer, int> compare)
        {
            cards.Sort((a1, a2) => { return compare(a1, a2); });
        }
    }
}