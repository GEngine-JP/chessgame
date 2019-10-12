using Sfs2X.Entities.Data;

namespace Assets.Scripts.Game.Mahjong3D.Standard
{
    public partial class GameLogic_Ting : AbsGameLogicBase
    {
        [S2CResponseHandler(NetworkProtocol.ResponseYoujin, GameMisc.BbmjKey)]
        public void OnTing_Youjin(ISFSObject data)
        {
            SetData(data);
            MahjongUtility.PlayOperateEffect(mData.CurrChair, PoolObjectType.youjin);
            Game.MahjongGroups.Do((groups) =>
            {
                int currChair = DataCenter.CurrOpChair;
                groups.MahjongHandWall[currChair].ThrowOut(mData.ThrowoutCard);
                var putCard = groups.MahjongThrow[currChair].GetInMahjong(mData.ThrowoutCard);
                Game.TableManager.ShowOutcardFlag(putCard);
                groups.MahjongHandWall[currChair].SetHandCardState(HandcardStateTyps.Ting);
            });
        }
    }
}