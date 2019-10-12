using System.Collections.Generic;

namespace Assets.Scripts.Game.Mahjong3D.Standard
{
    public partial class MahjongUtility
    {
        public static int GetMahjongCardAount(int card)
        {
            //花牌
            if (card >= (int)MahjongValue.ChunF && card <= (int)MahjongValue.JuF)
            {
                return 1;
            }
            else if (card == (int)MahjongValue.Baida)
            {
                return 4;
            }
            //正常牌
            return 4;
        }

        public static T GetItemByChair<T>(IList<T> list, int chair) where T : class
        {
            T item = null;
            if (chair >= list.Count) return null;
            int playerCount = GameCenter.DataCenter.MaxPlayerCount;
            switch (playerCount)
            {
                case 2:
                    {
                        if (chair == 0) { item = list[chair]; }
                        else { item = list[chair + 1]; }
                    };
                    break;
                case 3:
                    {
                        if (chair == 2) { item = list[chair + 1]; }
                        else { item = list[chair]; }
                    }
                    break;
                case 4: item = list[chair]; break;
            }
            return item;
        }

        /// <summary>
        /// 赖子检测
        /// </summary>     
        public static bool MahjongFlagCheck(int card)
        {
            int laizi = GameCenter.DataCenter.Game.LaiziCard;
            return MahjongFlagCheck(card, laizi);
        }

        /// <summary>
        /// 赖子检测
        /// </summary>   
        public static bool MahjongFlagCheck(int card, int laizi)
        {
            if (card == 0) return false;
            if (laizi == card) return true;
            bool flag = false;
            //判断赖子是春夏秋冬
            //if (laizi >= 96)
            //{
            //    if (card >= 96 && card < 100) flag = true;
            //    if (card >= 100 && card < 104) flag = true;
            //}
            return flag;
        }       

        /// <summary>
        /// 绝检测
        /// </summary>  
        public static Dictionary<bool, List<int>> MahjongFlagJueCheck(List<CpgData> cpgList, IList<int> handList, int huCard)
        {
            Dictionary<bool, List<int>> jueDic = new Dictionary<bool, List<int>>();
            bool huIsJue = false;
            List<int> jueList = new List<int>();
            List<int> canJueList = new List<int>();
            for (int i = 0; i < cpgList.Count; i++)
            {
                if (cpgList[i].Type == EnGroupType.Peng)
                {
                    canJueList.Add(cpgList[i].Card);
                    if (huCard == cpgList[i].Card)
                    {
                        jueList.Add(huCard);
                        huIsJue = true;
                    }
                }
            }
            for (int i = 0; i < canJueList.Count; i++)
            {
                for (int j = 0; j < handList.Count; j++)
                {
                    if (handList[j] == canJueList[i])
                    {
                        jueList.Add(canJueList[i]);
                        break;
                    }
                }
            }
            for (int i = 0; i < handList.Count - 2; i++)
            {
                if (handList[i] == handList[i + 1] && handList[i + 1] == handList[i + 2])
                {
                    if (i + 3 < handList.Count && handList[i + 2] == handList[i + 3])
                    {
                        jueList.Add(handList[i]);
                        i += 3;
                        continue;
                    }
                    canJueList.Add(handList[i]);
                    if (handList[i] == huCard) huIsJue = true;
                    i += 2;
                }
            }
            jueDic.Add(huIsJue, jueList);
            return jueDic;
        }
    }
}