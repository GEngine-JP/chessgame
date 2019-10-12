using Sfs2X.Entities.Data;

namespace Assets.Scripts.Game.Mahjong3D.Standard
{
    [S2CResponseLogic]
    public partial class GameLogic_ScoreDouble : AbsGameLogicBase
    {
        //加漂
        [S2CResponseHandler(NetworkProtocol.MJRequestTypeSelectPiao)]
        public void OnShowBottompour(ISFSObject data)
        {
            //游戏开始
            GameCenter.GameProcess.ChangeState<StateGamePlaying>();
            //显示UI
            GameCenter.EventHandle.Dispatch((int)UIEventProtocol.SetPlayerFlagState, new PlayerStateFlagArgs()
            {
                CtrlState = true,
                StateFlagType = (int)PlayerStateFlagType.Selecting
            });
            GameCenter.EventHandle.Dispatch((int)UIEventProtocol.ScoreDoubleCtrl);
        }

        //显示加漂分数
        [S2CResponseHandler(NetworkProtocol.MJRequestTypeShowPiao)]
        public void OnShowScoreDouble(ISFSObject data)
        {
            int[] array = data.GetIntArray("piaolist");
            int[] newArray = new int[array.Length];
            for (int i = 0; i < array.Length; i++)
            {
                int chair = MahjongUtility.GetChair(i);
                newArray[chair] = array[i];
            }
            ScoreDoubleArgs args = new ScoreDoubleArgs()
            {
                ScoreDoubleArray = newArray
            };
            var eventHandler = GameCenter.EventHandle;
            eventHandler.Dispatch((int)UIEventProtocol.ScoreDoubleCtrl, args);
            eventHandler.Dispatch((int)UIEventProtocol.SetPlayerFlagState, new PlayerStateFlagArgs() { CtrlState = false });
        }
    }
}