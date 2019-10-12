using Sfs2X.Entities.Data;

namespace Assets.Scripts.Game.Mahjong3D.Standard
{
    public partial class GameLogic_Ting : AbsGameLogicBase
    {
        [S2CResponseHandler(NetworkProtocol.MJRequestTypeDaiGu, GameMisc.NamjKey)]
        public void OnTing_Daigu(ISFSObject data)
        {
            SetData(data);
            LogicAction_Daigu();
        }

        /// <summary>
        /// 7对特殊听牌：自己有5对时，别人打牌的时候，正好组成第6对时允许听牌
        /// 把别人的手牌拿到自己手中，自己打一张牌
        /// </summary>
        private void LogicAction_Daigu()
        {
            Game.MahjongGroups.Do((groups) =>
            {
                int currChair = DataCenter.CurrOpChair;
                //播放特效
                MahjongUtility.PlayOperateEffect(currChair, PoolObjectType.ting);
                //打出一张手牌
                groups.MahjongHandWall[currChair].ThrowOut(mData.ThrowoutCard);
                //打出的牌显示再桌面
                var putCard = groups.MahjongThrow[currChair].GetInMahjong(mData.ThrowoutCard);
                Game.TableManager.ShowOutcardFlag(putCard);
                //把别人打的牌拿到手牌中               
                var otherCard = groups.MahjongThrow[DataCenter.OldOpChair].PopMahjong();
                int cardValue = otherCard.Value;
                groups.MahjongHandWall[currChair].GetInMahjongNoAni(cardValue);
                //切换听牌状态
                groups.MahjongHandWall[currChair].SetHandCardState(HandcardStateTyps.TingAndShowCard, new int[] { cardValue, cardValue });
            });
        }
    }
}