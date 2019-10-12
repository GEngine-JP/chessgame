using System.Collections.Generic;
using YxFramwork.ConstDefine;
using System;

namespace Assets.Scripts.Game.Mahjong3D.Standard
{
    public partial class C2SCustom
    {
        public void OnLaiZiGang()
        {
            List<int> laiZiGangList = FindLaiZiGang();
            Action<int> sendCall = (index) =>
            {
                Network.OnRequestC2S((sfs) =>
                {
                    sfs.PutInt(RequestKey.KeyType, NetworkProtocol.MJLaiZiGang);
                    sfs.PutInt(ProtocolKey.KeyTType,OperateKey.OperateLaiZiGang);
                    sfs.PutInt(RequestKey.KeyCard,laiZiGangList[index]);
                    return sfs;
                });
            };
            sendCall(0);
        }

        public List<int> FindLaiZiGang()
        {
            List<int> laiZiList = new List<int>();
            var dataCenter = GameCenter.DataCenter;
            int laizi = dataCenter.Game.LaiziCard;
            List<int> cards = dataCenter.OneselfData.HardCards;
            MahjongUtility.SortMahjong(cards);
            Dictionary<int, int> dic = GetCardAmount(cards);
            if (dic.ContainsKey(laizi))
            {
                laiZiList.Add(laizi);
            }
            return laiZiList;
        }
    }
}
