using YxFramwork.ConstDefine;
using Sfs2X.Entities.Data;

namespace Assets.Scripts.Game.Mahjong3D.Standard
{
    /// <summary>
    /// 抓牌数据
    /// </summary>
    public struct SendCardData : IData
    {
        public int Laizi;
        public int Fanpai;

        public void SetData(ISFSObject data)
        {
            var db = GameCenter.DataCenter;
            Fanpai = data.TryGetInt(ProtocolKey.CardFan);
            Laizi = data.TryGetInt(ProtocolKey.CardLaizi);
            db.Game.FanCard = Fanpai;
            db.Game.LaiziCard = Laizi;
            db.CurrOpSeat = data.TryGetInt(RequestKey.KeySeat);
            db.Players[0].HardCards.AddRange(data.GetIntArray(RequestKey.KeyCards));
        }
    }
}
