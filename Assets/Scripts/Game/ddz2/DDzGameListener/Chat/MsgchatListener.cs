using System;
using Assets.Scripts.Game.ddz2.DDz2Common;
using Assets.Scripts.Game.ddz2.DdzEventArgs;
using Assets.Scripts.Game.ddz2.InheritCommon;
using System.Collections.Generic;
using UnityEngine;
using YxFramwork.Common;
using YxFramwork.ConstDefine;

namespace Assets.Scripts.Game.ddz2.DDzGameListener.Chat
{
    /// <summary>
    /// 信息聊天系统listener
    /// </summary>
    public class MsgchatListener : ServEvtListener
    {
        /// <summary>
        /// 自己的聊天信息表达
        /// </summary>
        [SerializeField]
        protected ExpressionCtrl SelfExpCtrl;

        /// <summary>
        /// 右侧玩家聊天信息表达
        /// </summary>
        [SerializeField]
        protected ExpressionCtrl LeftExpCtrl;

        /// <summary>
        /// 左侧玩家聊天信息的表达
        /// </summary>
        [SerializeField]
        protected ExpressionCtrl RightExpCtrl;

        /// <summary>
        /// 聊天系统ui
        /// </summary>
        [SerializeField]
        protected MsgChatUiCtrl MsgChatuictrl;


        [SerializeField] protected AudioClip[] ClipSource;

        /// <summary>
        /// 某段聊天文字对应的音效
        /// </summary>
        Dictionary<string,AudioClip> _strtoclipDic = new Dictionary<string,AudioClip>();

        protected override void OnAwake()
        {
            Ddz2RemoteServer.AddOnMsgChatEvt(OnUserTalk);
        }

        private void Start()
        {
            //如果声源和文字数组长度对应说明有问题
            if (ClipSource == null || ClipSource.Length != MsgChatuictrl.TalkStrs.Length)
            {
                throw new Exception("快接语音和声源数量不一致");
            }

            var talkStrs = MsgChatuictrl.TalkStrs;
            var len = ClipSource.Length;
            for (var i = 0; i < len; i++)
            {
                _strtoclipDic[talkStrs[i]] = ClipSource[i];
            }
        }


        private void OnUserTalk(object sender, DdzbaseEventArgs args)
        {
            var data = args.IsfObjData;

            int seat = data.GetInt(RequestKey.KeySeat);
            int exp = data.GetInt(RequestKey.KeyExp);
            string msgtext = data.GetUtfString(RequestKey.KeyText);

            if (seat == App.GetGameData<GlobalData>().GetSelfSeat)
            {
                ShowTalk(SelfExpCtrl, exp, msgtext);
            }
            else if (seat == App.GetGameData<GlobalData>().GetRightPlayerSeat)
            {
                ShowTalk(RightExpCtrl, exp, msgtext);
            }
            else if (seat == App.GetGameData<GlobalData>().GetLeftPlayerSeat)
            {
                ShowTalk(LeftExpCtrl, exp, msgtext);
            }
        }

        private void ShowTalk(ExpressionCtrl expctrl,int exp, string msgtext)
        {
            if (string.IsNullOrEmpty(msgtext))
            {
                expctrl.ShowExp(exp);
                return;
            }

            AudioClip audioClip = null;
            if (_strtoclipDic.ContainsKey(msgtext)) audioClip = _strtoclipDic[msgtext];

            expctrl.ShowText(msgtext, 3, audioClip);
        }


        public override void RefreshUiInfo()
        {
                          
        }
    }
}
