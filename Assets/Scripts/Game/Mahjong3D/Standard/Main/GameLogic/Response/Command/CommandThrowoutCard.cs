namespace Assets.Scripts.Game.Mahjong3D.Standard
{
    public class CommandThrowoutCard : AbsGameCommand<ActionThrowoutCard>
    {
        [PlaybackHandlerAttrubute(PlaybackProtocol.ThrowOut)]
        public void PlaybackThrowoutCard(PlaybackFrameData data)
        {
            LogicAction.PlaybackThrowoutCard(data);
        }
    }
}
