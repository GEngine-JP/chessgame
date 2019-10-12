using Sfs2X.Entities.Data;

namespace Assets.Scripts.Game.Mahjong3D.Standard
{
    public class CommandSendCard : AbsGameCommand<ActionSendCard>
    {
        [PlaybackHandlerAttrubute(PlaybackProtocol.Allowcate)]
        public void PlaybackSendCard(PlaybackFrameData data)
        {
            LogicAction.PlaybackSendCard(data);
        }
    }
}