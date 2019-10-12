namespace Assets.Scripts.Game.Mahjong3D.Standard
{
    public partial class CommandCpg : AbsGameCommand<ActionCpg>
    {

    }

    public class CommandCpgPlayback : AbsGameCommand<ActionCpgPlayback>
    {
        [PlaybackHandlerAttrubute(PlaybackProtocol.Chi)]
        public void PlaybackChi(PlaybackFrameData data)
        {
            LogicAction.PlaybackChi(data);
        }

        [PlaybackHandlerAttrubute(PlaybackProtocol.Peng)]
        public void PlaybackPeng(PlaybackFrameData data)
        {
            LogicAction.PlaybackPeng(data);
        }

        [PlaybackHandlerAttrubute(PlaybackProtocol.Gang_Zhua)]
        public void PlaybackGangZhua(PlaybackFrameData data)
        {
            LogicAction.PlaybackZhuaGang(data);
        }

        [PlaybackHandlerAttrubute(PlaybackProtocol.Gang_Ming)]
        public void PlaybackGangMing(PlaybackFrameData data)
        {
            LogicAction.PlaybackMingGang(data);
        }

        [PlaybackHandlerAttrubute(PlaybackProtocol.Gang_An)]
        public void PlaybackAnGang(PlaybackFrameData data)
        {
            LogicAction.PlaybackAnGang(data);
        }
    }
}
