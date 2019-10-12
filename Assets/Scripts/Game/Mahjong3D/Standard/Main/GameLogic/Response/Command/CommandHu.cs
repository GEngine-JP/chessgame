namespace Assets.Scripts.Game.Mahjong3D.Standard
{
    public partial class CommandHu : AbsGameCommand<ActionHu>
    {

    }

    public class CommandHuPlayback : AbsGameCommand<ActionHuPlayback>
    {
        [PlaybackHandlerAttrubute(PlaybackProtocol.Hu)]
        public void PlaybackHu(PlaybackFrameData data)
        {
            LogicAction.PlaybackHu(data);
        }

        [PlaybackHandlerAttrubute(PlaybackProtocol.Zimo)]
        public void PlaybackZimo(PlaybackFrameData data)
        {
            LogicAction.PlaybackZimo(data);
        }

        [PlaybackHandlerAttrubute(PlaybackProtocol.ZhaNiao)]
        public void PlaybackZhaNiao(PlaybackFrameData data)
        {
            LogicAction.PlaybackZhaNiao(data);
        }

        [PlaybackHandlerAttrubute(PlaybackProtocol.GameOver)]
        public void PlaybackGameOver(PlaybackFrameData data)
        {
            LogicAction.PlaybackGameOver(data);
        }

        [PlaybackHandlerAttrubute(PlaybackProtocol.LiuJu)]
        public void PlaybackLiuJu(PlaybackFrameData data)
        {
            LogicAction.PlaybackLiuju(data);
        }

        [PlaybackHandlerAttrubute(PlaybackProtocol.Ting)]
        public void PlaybackTing(PlaybackFrameData data)
        {
            LogicAction.PlaybackTing(data);
        }
    }
}
