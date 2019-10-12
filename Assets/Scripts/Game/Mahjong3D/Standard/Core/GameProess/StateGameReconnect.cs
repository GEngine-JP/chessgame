﻿using YxFramwork.ConstDefine;

namespace Assets.Scripts.Game.Mahjong3D.Standard
{
    public class StateGameReconnect : FsmState
    {
        public override void OnEnter(FsmStateArgs args)
        {
            //执行继承IReconnectedCycle接口脚本
            GameCenter.Lifecycle.ReconnectedCycle();
            var gameinfoData = args as SfsFsmStateArgs;
            var handler = GameCenter.Network.DispatchResponseHandlers(CustomClientProtocol.CustomTypeReconnectLogic);
            if (null != handler)
            {
                handler(gameinfoData.SFSObject);
            }
            if (GameCenter.DataCenter.OperateMenu != 0)
            {
                var opHandler = GameCenter.Network.DispatchResponseHandlers(NetworkProtocol.MJOpreateType);
                if (null != opHandler)
                {
                    var userSFSObject = gameinfoData.SFSObject.GetSFSObject(RequestKey.KeyUser);
                    opHandler(userSFSObject);
                }
            }
            ChangeState<StateGamePlaying>();
        }

        public override void OnLeave(bool isShutdown) { }
    }
}