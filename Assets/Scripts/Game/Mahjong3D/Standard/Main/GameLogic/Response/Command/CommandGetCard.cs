namespace Assets.Scripts.Game.Mahjong3D.Standard
{
    public class CommandGetCard : AbsGameCommand<ActionGetCard>
    {
        [PlaybackHandlerAttrubute(PlaybackProtocol.GetIn)]
        public void PlaybackGetCard(PlaybackFrameData data)
        {
            LogicAction.PlaybackGetCard(data);
        }
    }
}
