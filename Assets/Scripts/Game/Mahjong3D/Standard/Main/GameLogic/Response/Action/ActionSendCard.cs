namespace Assets.Scripts.Game.Mahjong3D.Standard
{
    public class ActionSendCard : AbsCommandAction
    {
        public void PlaybackSendCard(PlaybackFrameData data)
        {
            Game.MahjongGroups.MahjongHandWall[data.OpChair].GetInMahjong(data.Cards);
        }
    }
}