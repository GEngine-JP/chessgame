using Sfs2X.Entities.Data;

namespace Assets.Scripts.Game.Mahjong3D.Standard
{
    public partial class GameLogic_Cpg : AbsGameLogicBase
    {
        [S2CResponseHandler(NetworkProtocol.MJRequestTypeCPG, GameKey = GameMisc.WmbbmjKey)]
        [S2CResponseHandler(NetworkProtocol.MJRequestTypeSelfGang, GameKey = GameMisc.WmbbmjKey)]
        public void OnResponseCpg_Wmbbmj(ISFSObject data)
        {
            OnResponseCpg(data);

            //财飘 暗杠
            if (mData.CpgData.Type == EnGroupType.AnGang
                || mData.CpgData.Type == EnGroupType.MingGang
                || mData.CpgData.Type == EnGroupType.ZhuaGang)
            {
                var throwoutCard = GameCenter.Network.GetGameResponseLogic<GameLogic_ThrowoutCard>();
                var flag = throwoutCard.CheckCaipiaoState();
                if (flag)
                {
                    var playerHand = Game.MahjongGroups.MahjongHandWall[0].GetMahHandComponent<MahPlayerHand>();
                    playerHand.SwitchFreezeState();
                }
            }
        }
    }
}
