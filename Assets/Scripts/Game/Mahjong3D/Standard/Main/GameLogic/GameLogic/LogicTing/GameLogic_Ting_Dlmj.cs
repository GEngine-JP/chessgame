using Sfs2X.Entities.Data;

namespace Assets.Scripts.Game.Mahjong3D.Standard
{
    public partial class GameLogic_Ting : AbsGameLogicBase
    {
        [S2CResponseHandler(NetworkProtocol.MJRequestTypeTing, GameMisc.ZhmjKey)]
        [S2CResponseHandler(NetworkProtocol.MJRequestTypeTing, GameMisc.DlmjKey)]
        [S2CResponseHandler(NetworkProtocol.MJRequestTypeTing, GameMisc.DltdhKey)]
        [S2CResponseHandler(NetworkProtocol.MJRequestTypeTing, GameMisc.TdDltdhKey)]
        public void OnTing_DaLian(ISFSObject data)
        {
            mData.SetData(data);
            int currChair = DataCenter.CurrOpChair;
            DataCenter.Players[currChair].IsAuto = true;
            MahjongUtility.PlayOperateEffect(currChair, PoolObjectType.ting);
            GameCenter.EventHandle.Dispatch((int)UIEventProtocol.OnTing, new OnTingArgs() { TingChair = currChair });
            var niuTings = data.ContainsKey("niu") ? data.GetIntArray("niu") : null;

            LogicAction_DaLian(niuTings);
        }

        /// <summary>
        /// 特殊牛听牌,必须要胡夹
        /// </summary>
        private void LogicAction_DaLian(int[] niuTings)
        {
            Game.MahjongGroups.Do((groups) =>
            {
                int currChair = DataCenter.CurrOpChair;
                //打出一张手牌
                groups.MahjongHandWall[currChair].ThrowOut(mData.ThrowoutCard);
                //打出的牌显示再桌面
                var putCard = groups.MahjongThrow[currChair].GetInMahjong(mData.ThrowoutCard);
                Game.TableManager.ShowOutcardFlag(putCard);
                if (niuTings == null || niuTings.Length < 1)
                {
                    //切换听牌状态
                    groups.MahjongHandWall[currChair].SetHandCardState(HandcardStateTyps.Ting);
                }
                else
                {
                    //切换牛听牌状态
                    groups.MahjongHandWall[currChair].SetHandCardState(HandcardStateTyps.TingAndShowCard, niuTings);
                }
            });
        }
    }
}