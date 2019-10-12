using System;
using YxFramwork.ConstDefine;

namespace Assets.Scripts.Game.Mahjong3D.Standard
{
    public partial class C2SCustom
    {
        public void OnJueGnag()
        {
            var dataCenter = GameCenter.DataCenter;
            var cards = dataCenter.OneselfData.HardCards;
            var fanCard = dataCenter.Game.FanCard;

            MahjongUtility.SortMahjong(cards);
            var dic = GetCardAmount(cards);

            Action<int[]> sendCall = (arr) =>
            {
                Network.OnRequestC2S((sfs) =>
                {
                    sfs.PutInt(RequestKey.KeyType, NetworkProtocol.MJRequestJueGang);
                    sfs.PutInt(ProtocolKey.KeyTType, 11);
                    sfs.PutIntArray(RequestKey.KeyCards, arr);
                    return sfs;
                });
            };

            Action<int[]> sendCallPeng = (arr) =>
            {
                Network.OnRequestC2S((sfs) =>
                {
                    sfs.PutInt(RequestKey.KeyType, NetworkProtocol.MJRequestTypeCPG);
                    sfs.PutInt(ProtocolKey.KeyTType, 13);
                    sfs.PutIntArray(RequestKey.KeyCards, arr);
                    return sfs;
                });
            };

            if (dataCenter.OneselfData.Chair == dataCenter.CurrOpChair)
            {
                if (dic.ContainsKey(fanCard) && dic[fanCard] == 3)
                {
                    sendCall(new[] { fanCard, fanCard, fanCard });
                }
            }
            else
            {
                if (dic.ContainsKey(fanCard) && dic[fanCard] == 2)
                {
                    sendCallPeng(new[] { fanCard, fanCard });
                }
            }
        }
    }
}