using System.Collections.Generic;
using YxFramwork.Framework.Core;
using Sfs2X.Entities.Data;
using YxFramwork.Manager;
using UnityEngine;

namespace Assets.Scripts.Game.Mahjong3D.Standard
{
    public class StateGameInfoInit : FsmState
    {
        public override void OnEnter(FsmStateArgs args)
        {
            var dataCenter = GameCenter.DataCenter;
            if (dataCenter.Room.RoomType == MahRoomType.YuLe)
            {
                GameCenter.Network.CtrlYuleRejoin(true);
            }

            //执行继承IGameInfoICycle接口脚本
            GameCenter.Lifecycle.GameInfoICycle();
            //检查投票解散房间
            ContinueTaskManager.NewTask().AppendFuncTask(() => RejionHandup((args as SfsFsmStateArgs).SFSObject)).Start();
            //var flag = dataCenter.Players.IsSende() && (dataCenter.Room.RoomType == MahRoomType.YuLe);
            if (dataCenter.IsReconect)
            {
                ChangeState<StateGameReconnect>(args);
            }
            else
            {
                ChangeState<StateGameReady>();
                InviteLastMahjongFriend();
            }
            SetShake();
        }

        /// <summary>
        /// 设置手机震动
        /// </summary>
        private void SetShake()
        {
            int shakeCtrl = MahjongUtility.ShakeCtrl;
            if (shakeCtrl == (int)CtrlSwitchType.None)
            {
                if (GameCenter.DataCenter.Config.MobileShake)
                {
                    MahjongUtility.ShakeCtrl = (int)CtrlSwitchType.Open;
                }
                else
                {
                    MahjongUtility.ShakeCtrl = (int)CtrlSwitchType.Close;
                }
            }
            else
            {
                MahjongUtility.ShakeCtrl = shakeCtrl;
            }
        }

        /// <summary>
        /// 如果是继续开局，邀请上一局麻友
        /// </summary>
        private void InviteLastMahjongFriend()
        {
            var db = GameCenter.DataCenter;
            if (db.Room.RoomType == MahRoomType.FanKa)
            {
                PlayerPrefs.SetString(Keys.TeaId, "");
                if (!string.IsNullOrEmpty(MahjongUtility.RecordCountineGameData))
                {
                    //自己是房主，邀请麻友
                    if (db.OneselfData.IsOwner)
                    {
                        ContinueTaskManager.NewTask().AppendFuncTask(OnInviteFriend).Start();
                    }
                }
            }
        }

        /// <summary>
        /// 邀请麻友
        /// </summary>        
        private IEnumerator<float> OnInviteFriend()
        {
            var db = GameCenter.DataCenter;
            string[] ids = MahjongUtility.RecordCountineGameData.Split('|');
            if (ids != null || ids.Length > 0)
            {
                for (int i = 0; i < ids.Length; i++)
                {
                    string id = ids[i];
                    if (!string.IsNullOrEmpty(id) && db.OneselfData.Id != int.Parse(id))
                    {
                        var apiInfo = new Dictionary<string, object>()
                        {
                            { "bundleID", Application.bundleIdentifier },
                            { "roomId", db.Room.RoomID },
                            { "inviteId", id }
                        };
                        Facade.Instance<TwManager>().SendAction("mahjongwm.inviteWmFriends", apiInfo, data => { });
                    }
                    yield return 1.5f;
                }
                //清空数据
                MahjongUtility.RecordCountineGameData = string.Empty;
            }
        }

        /// <summary>
        /// 投票解散
        /// </summary>
        private IEnumerator<float> RejionHandup(ISFSObject data)
        {
            var db = GameCenter.DataCenter;
            if (data.ContainsKey("hupstart"))
            {
                yield return 0.3f;
                int handUpTime = (int)db.Config.TimeHandup;
                int time = handUpTime - (int)(data.TryGetLong("hupnow") - data.TryGetLong("hupstart"));
                if (time > handUpTime)
                {
                    time = handUpTime;
                }
                string[] strArr = data.TryGetString("hup").Split(',');
                for (int i = 0; i < strArr.Length; i++)
                {
                    for (int j = 0; j < db.MaxPlayerCount; j++)
                    {
                        if (db.Players[j].Id == int.Parse(strArr[i]))
                        {
                            HandupEventArgs args = new HandupEventArgs()
                            {
                                Chair = j,
                                Time = time,
                                HandupType = i == 0 ? DismissFeedBack.ApplyFor : DismissFeedBack.Agree
                            };
                            GameCenter.EventHandle.Dispatch((int)UIEventProtocol.OnEventHandUp, args);
                        }
                    }
                }
            }
        }

        public override void OnLeave(bool isShutdown) { }
    }
}