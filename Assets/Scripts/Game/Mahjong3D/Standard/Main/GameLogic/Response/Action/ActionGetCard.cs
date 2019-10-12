namespace Assets.Scripts.Game.Mahjong3D.Standard
{
    public class ActionGetCard : AbsCommandAction
    {
        public void PlaybackGetCard(PlaybackFrameData data)
        {
            Game.MahjongGroups.MahjongHandWall[data.OpChair].GetInMahjong(data.Cards[0]);
            MahjongUtility.PlayEnvironmentSound("zhuapai");
        }
    }
}
