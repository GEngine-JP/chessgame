using YxFramwork.ConstDefine;
using Sfs2X.Entities.Data;

namespace Assets.Scripts.Game.Mahjong3D.Standard
{
    [S2CResponseLogic]
    public partial class GameLogic_Ting : AbsGameLogicBase
    {
        private TingData mData;

        [S2CResponseHandler(NetworkProtocol.MJRequestTypeTing)]
        public void OnTing(ISFSObject data)
        {
            TingLogic(data);
            MahjongUtility.PlayOperateEffect(mData.CurrChair, PoolObjectType.ting);
        }

        public void TingLogic(ISFSObject data)
        {
            SetData(data);
            Game.MahjongGroups.Do((groups) =>
            {
                groups.MahjongHandWall[mData.CurrChair].ThrowOut(mData.ThrowoutCard);
                groups.MahjongHandWall[mData.CurrChair].SetHandCardState(HandcardStateTyps.Ting);
                var item = groups.MahjongThrow[mData.CurrChair].GetInMahjong(mData.ThrowoutCard);
                Game.TableManager.ShowOutcardFlag(item);
            });
        }

        private void SetData(ISFSObject data)
        {
            mData.SetData(data);
            DataCenter.Players[mData.CurrChair].IsAuto = true;
            GameCenter.EventHandle.Dispatch((int)UIEventProtocol.OnTing, new OnTingArgs() { TingChair = mData.CurrChair });
        }
    }

    /// <summary>
    /// Ting数据
    /// </summary>
    public struct TingData : IData
    {
        public int CurrChair;
        public int ThrowoutCard;

        public void SetData(ISFSObject data)
        {
            var db = GameCenter.DataCenter;
            ThrowoutCard = data.GetInt(RequestKey.KeyOpCard);
            db.CurrOpSeat = data.GetInt(RequestKey.KeySeat);
            db.ThrowoutCard = ThrowoutCard;
            CurrChair = db.CurrOpChair;
            GameCenter.Shortcuts.MahjongQuery.Do(p => p.ShowQueryTip(null));
        }
    }
}