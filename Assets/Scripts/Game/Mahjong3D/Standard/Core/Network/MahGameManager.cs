using Sfs2X.Entities.Data;
using YxFramwork.Manager;
using System;

namespace Assets.Scripts.Game.Mahjong3D.Standard
{
    public class MahGameManager : YxGameManager
    {
        //退出
        public event Action<int, ISFSObject> UserOutEvent;
        //准备
        public event Action<int, ISFSObject> UserReadyEvent;
        //切换状态
        public event Action<int, ISFSObject> GameStateEvent;
        //接受服务器响应消息
        public event Action<int, ISFSObject> GameResponseEvent;
        //其他玩家加入房间
        public event Action<ISFSObject> OnOtherPlayerJoinRoomEvent;
        //获取游戏数据
        public event Action<ISFSObject> OnGetGameInfoEvent;

        public override void GameResponseStatus(int type, ISFSObject response)
        {
            GameResponseEvent(type, response);
        }

        public override void UserOut(int localseat, ISFSObject response)
        {
            UserOutEvent(localseat, response);
        }

        public override void UserReady(int localseat, ISFSObject response)
        {
            base.UserReady(localseat, response);
            UserReadyEvent(localseat, response);
        }

        public override void OnGetGameInfo(ISFSObject gameInfo)
        {
            OnGetGameInfoEvent(gameInfo);
        }

        public override void OnGetRejoinInfo(ISFSObject gameInfo)
        {
            GameCenter.Network.RefreshResponseQueue();
        }

        public override void GameStatus(int status, ISFSObject info)
        {
            GameStateEvent(status, info);
        }

        public override void OnOtherPlayerJoinRoom(ISFSObject data)
        {
            OnOtherPlayerJoinRoomEvent(data);
        }

        public override int OnChangeRoom()
        {
            // 娱乐房，切换行间 清理所有消息 设置标志位
            GameCenter.DataCenter.Room.YuLeBoutState = true;
            GameCenter.Network.CtrlYuleRejoin(false);
            GameCenter.Network.RefreshResponseQueue();
            return base.OnChangeRoom();
        }
    }
}