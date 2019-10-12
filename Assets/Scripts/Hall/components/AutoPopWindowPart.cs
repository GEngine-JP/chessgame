using System;
using System.Collections.Generic;
using UnityEngine;
using YxFramwork.Common;
using YxFramwork.Common.Model;
using YxFramwork.Common.Utils;
using YxFramwork.Enums;
using YxFramwork.Framework.Core;
using YxFramwork.Manager;

namespace Assets.Scripts.Hall.components
{
    /// <summary>
    /// �Զ������������
    /// </summary>
    public class AutoPopWindowPart : MonoBehaviour
    {
        /// <summary>
        /// �Զ��򿪵Ĵ���
        /// </summary>
        public string[] AutoPopWindowNames;

        private static bool _hasPop;


        private void Awake()
        {
            Facade.EventCenter.AddEventListeners<string,object>("HallWindow_hallMenuChange", ShowAutoPopWindows);
        }

        /// <summary>
        /// �Զ��򿪴���
        /// </summary>
        private void ShowAutoPopWindows(object msg)
        {
            if (_hasPop) return;
            _hasPop = true;
            var autoState = App.AppStyle == YxEAppStyle.Concise ? 0 : HallModel.Instance.OptionSwitch.AutoPopWin;
            var len = AutoPopWindowNames.Length;
            for (var i = 0; i < len; i++)
            {
                var show = 1 << i;
                if ((autoState & show) != show) continue;

                var winName = AutoPopWindowNames[i];
                if (CheckWindowNeedOpen(winName))
                {
                    YxWindowManager.OpenWindow(winName, true, null, null, "SpreadWindow".Equals(winName));
                }
            }
        }

        /// <summary>
        /// ����Ƿ�򿪴���
        /// </summary>
        /// <param name="winName"></param>
        /// <returns></returns>
        private bool CheckWindowNeedOpen(string winName)
        {
            if (string.IsNullOrEmpty(winName)) { return false; }
            var key = string.Format("AutoWindow_{0}_{1}", winName, App.UserId);
            switch (winName)
            {
                case "ActionNoticeQueueWindow":
                    var curDate = DateTime.Now.ToString("MM/dd/yyyy");
                    if (curDate == Util.GetString(key))
                    {
                        return false;
                    }
                    Util.SetString(key, curDate);
                    return true;
                case "SpreadWindow":
                    var promoter = UserInfoModel.Instance.UserInfo.Promoter;
                    if (promoter != false)
                    {
                        var times = Util.GetInt(key);
                        if (times > 3)
                        {
                            return false;
                        }
                        Util.SetInt(key, ++times);
                    }
                    return true;
                default:
                    return true;
            }
        }
    }
}
