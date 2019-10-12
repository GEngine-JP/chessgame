using Sfs2X.Entities.Data;
using YxFramwork.ConstDefine;

namespace Assets.Scripts.Game.Mahjong3D.Standard
{
    public partial class GameLogic_Ting : AbsGameLogicBase
    {
        [S2CResponseHandler(NetworkProtocol.MJRequestTypeLiangDao, GameMisc.QdjtKey)]
        public void OnTing_Qdjt(ISFSObject data)
        {
            mData.SetData(data);
            var currChair = DataCenter.CurrOpChair;
            var cards = data.TryGetIntArray(RequestKey.KeyCards);

            var panel = GameCenter.Hud.GetPanel<PanelOtherHuTip>();
            panel.Open(cards, currChair);
            LogicActionLiangdao(cards);

            DataCenter.Players[currChair].IsAuto = true;
            MahjongUtility.PlayOperateEffect(currChair, PoolObjectType.liangdao);
            GameCenter.EventHandle.Dispatch((int)UIEventProtocol.OnTing, new OnTingArgs() { TingChair = currChair });
        }

        /// <summary>
        /// 亮倒
        /// </summary>
        private void LogicActionLiangdao(int[] liangCards)
        {
            if (liangCards == null || liangCards.Length < 1) return;
            Game.MahjongGroups.Do((groups) =>
            {
                int currChair = DataCenter.CurrOpChair;
                //打出一张手牌
                //groups.MahjongHandWall[currChair].ThrowOut(mData.ThrowoutCard);
                //打出的牌显示再桌面
                //var putCard = groups.MahjongThrow[currChair].GetInMahjong(mData.ThrowoutCard);
                //Game.TableManager.ShowOutcardFlag(putCard);
                //亮牌
                groups.MahjongHandWall[currChair].SetHandCardState(HandcardStateTyps.TingAndShowCard, liangCards);
            });
        }
    }
}
