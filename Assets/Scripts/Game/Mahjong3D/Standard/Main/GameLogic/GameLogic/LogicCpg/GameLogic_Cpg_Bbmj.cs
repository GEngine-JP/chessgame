using Sfs2X.Entities.Data;

namespace Assets.Scripts.Game.Mahjong3D.Standard
{
    public partial class GameLogic_Cpg : AbsGameLogicBase
    {
        [S2CResponseHandler(NetworkProtocol.MJRequestTypeCPG, GameKey = GameMisc.BbmjKey)]
        [S2CResponseHandler(NetworkProtocol.MJRequestTypeSelfGang, GameKey = GameMisc.BbmjKey)]
        public void OnResponseCpg_Bbmj(ISFSObject data)
        {
            CpgLogic(data);
            switch (mData.CpgType)
            {
                case EnGroupType.Chi:
                    MahjongUtility.PlayOperateEffect(DataCenter.CurrOpChair, PoolObjectType.chi);
                    break;
                case EnGroupType.Peng:
                    MahjongUtility.PlayOperateEffect(DataCenter.CurrOpChair, PoolObjectType.peng);
                    break;
                default:
                    string soundName = "gang";
                    if (mData.CpgType == EnGroupType.MingGang)
                    {
                        soundName = "minggang";
                    }
                    else if (mData.CpgType == EnGroupType.AnGang)
                    {
                        soundName = "angang";
                    }
                    MahjongUtility.PlayPlayerSound(mData.CurrOpChair, soundName);
                    MahjongUtility.PlayOperateEffect(mData.CurrOpChair, PoolObjectType.gang);
                    MahjongUtility.PlayEnvironmentEffect(mData.CurrOpChair, PoolObjectType.longjuanfeng);
                    PlayScoreEffect(data);
                    break;
            }
        }
    }
}