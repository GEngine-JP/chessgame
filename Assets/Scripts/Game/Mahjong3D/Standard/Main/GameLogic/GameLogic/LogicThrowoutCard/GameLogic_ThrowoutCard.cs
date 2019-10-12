using YxFramwork.ConstDefine;
using Sfs2X.Entities.Data;

namespace Assets.Scripts.Game.Mahjong3D.Standard
{
    [S2CResponseLogic]
    public partial class GameLogic_ThrowoutCard : AbsGameLogicBase
    {
        private ThrowoutCardData mData;

        [S2CResponseHandler(NetworkProtocol.MJThrowoutCard)]
        public void OnThrowoutCard(ISFSObject data)
        {
            mData.SetData(data);
            LogicAction();
        }

        public void LogicAction()
        {
            int currChair = DataCenter.CurrOpChair;
            Game.MahjongGroups.Do((groups) =>
            {
                groups.OnClearFlagMahjong();
                if (DataCenter.CurrOpChair == 0)
                {
                    UserContorl.ClearSelectCard();
                }
                groups.MahjongHandWall[currChair].ThrowOut(mData.ThrowoutCard);
                var item = groups.MahjongThrow[currChair].GetInMahjong(mData.ThrowoutCard);
                Game.TableManager.ShowOutcardFlag(item);
            });
            MahjongUtility.PlayMahjongSound(currChair, mData.ThrowoutCard);

            //金币房时 玩家选择听牌时，如果时间到了系统自动出牌， 恢复为正常出牌状态
            if (DataCenter.Room.RoomType == MahRoomType.YuLe)
            {
                var playerHand = Game.MahjongGroups.PlayerHand.GetMahHandComponent<MahPlayerHand>();
                if (playerHand.CurrState == HandcardStateTyps.ChooseNiuTing || playerHand.CurrState == HandcardStateTyps.ChooseTingCard)
                {
                    playerHand.SetHandCardState(HandcardStateTyps.Normal);
                }
            }
        }

        /// <summary>
        /// 跟庄
        /// </summary>
        /// <param name="data"></param>
        private void OnGenzhuang(ISFSObject data)
        {
            if (data.ContainsKey("genZhuang"))
            {
                var gold = data.GetIntArray("genZhuang");
                var chairGold = new int[gold.Length];
                for (int i = 0; i < gold.Length; i++)
                {
                    var chair = MahjongUtility.GetChair(i);
                    chairGold[chair] = gold[i];
                }
            }
        }
    }

    /// <summary>
    /// 出牌数据
    /// </summary>
    public struct ThrowoutCardData : IData
    {
        public int PlayerSeat;
        public int ThrowoutCard;

        public void SetData(ISFSObject data)
        {
            var db = GameCenter.DataCenter;
            PlayerSeat = data.GetInt(RequestKey.KeySeat);
            ThrowoutCard = data.GetInt(RequestKey.KeyOpCard);
            db.CurrOpSeat = PlayerSeat;
            db.ThrowoutCard = ThrowoutCard;
            GameCenter.Shortcuts.MahjongQuery.Do(p => p.ShowQueryTip(null));
            if (db.CurrOpChair == 0)
            {
                GameCenter.EventHandle.Dispatch((int)UIEventProtocol.QueryHuCard, new QueryHuArgs() { PanelState = false });
            }
        }
    }
}