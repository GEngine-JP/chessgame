namespace Assets.Scripts.Game.Mahjong3D.Standard
{
    public class CommandLaizi : AbsGameCommand<ActionLaizi>
    {
        [PlaybackHandlerAttrubute(PlaybackProtocol.Laizi)]
        public void PlaybackLaizi(PlaybackFrameData data)
        {
            LogicAction.PlaybackLaizi(data);
        }

        [PlaybackHandlerAttrubute(PlaybackProtocol.HuanBao)]
        public void PlaybackHuanBao(PlaybackFrameData data)
        {
            LogicAction.PlaybackBao(data);
        }

        [PlaybackHandlerAttrubute(PlaybackProtocol.FanPai)]
        public void PlaybackFanPai(PlaybackFrameData data)
        {
            LogicAction.PlaybackFanPai(data);
        }
    }
}
