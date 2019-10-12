namespace Assets.Scripts.Game.Mahjong3D.Standard
{
    public class StateGameReady : FsmState
    {
        public override void OnEnter(FsmStateArgs args)
        {
            if (GameCenter.DataCenter.Room.RoomType == MahRoomType.YuLe)
            {
                GameCenter.Network.CtrlYuleRejoin(true);
            }

            //执行继承IGameEndCycle接口脚本
            GameCenter.Lifecycle.GameEndCycle();
            //执行继承IGameReadyCycle接口脚本
            GameCenter.Lifecycle.GameReadyCycle();
            var db = GameCenter.DataCenter;
            //当前用户没有准被 发送准备消息
            if (!db.OneselfData.IsReady)
            {
                if (db.ConfigData.AutoReady)
                {
                    GameCenter.Network.C2S.Ready();
                }
                else
                {
                    GameCenter.EventHandle.Dispatch((int)UIEventProtocol.ReadyBtnCtrl, new PanelTriggerArgs() { ReadyState = true });
                }
            }
            var handler = GameCenter.Network.DispatchResponseHandlers(CustomClientProtocol.CustomTypeReadyLogic);
            if (null != handler)
            {
                handler(null);
            }
        }

        public override void OnLeave(bool isShutdown) { }
    }
}