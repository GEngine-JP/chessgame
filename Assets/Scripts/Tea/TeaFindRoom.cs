﻿using System;
using System.Collections.Generic;
using Assets.Scripts.Common.Windows;
using Assets.Scripts.Hall.View.AboutRoomWindows;
using UnityEngine;
using YxFramwork.Common.Utils;
using YxFramwork.Framework;
using YxFramwork.Framework.Core;
using YxFramwork.Manager;
using YxFramwork.View;

namespace Assets.Scripts.Tea
{
    public class TeaFindRoom : YxNguiWindow
    {
        /// <summary>
        /// 房间id的Label
        /// </summary>
        [Tooltip("房间id的Label")]
        public UILabel RoomIdLabel;
        /// <summary>
        /// 房间id最大位数
        /// </summary>
        [Tooltip("房间id最大位数")]
        public int MaxIdCount = 6;
        /// <summary>
        /// 查找消息窗口
        /// </summary>
        [Tooltip("输入键")]
        public RoomInfoWindow RoominfoWindow;
        /// <summary>
        /// 房间id最大位数
        /// </summary>
        [Tooltip("输入键")]
        public UIButton[] Keyboards;
        /// <summary>
        /// 标签默认索引
        /// </summary>
        public int TabDefaultIndex = -1;
        [Tooltip("显示输入内容(多Label)")]
        public List<UILabel> RoomIdMoreLabel=new List<UILabel>();
        [Tooltip("存储对应玩家茶馆Id的Key")]
        public string KeyTeaId = "TeaId";
        [Tooltip("自动查找")]
        public bool AutoFind = true;
        /// <summary>
        /// 当前茶馆功能的命名 可以叫俱乐部 亲友圈 自己填写
        /// </summary>
        public string CurrentName = "茶馆";
        protected void Awake()
        {
            var count = Keyboards.Length;
            for (var i = 0; i < count; i++)
            {
                var btn = Keyboards[i];
                if (btn == null) continue;
                UIEventListener.Get(btn.gameObject).onClick = OnClick;
            }
        }

        public void OnClick(GameObject go)
        {
            OnClickWithName(go.name);
        }

        public void OnClickWithName(string btnName)
        {
            switch (btnName)
            {
                case "del":
                    Delete();
                    break;
                case "cle":
                    Clear();
                    break;
                default:
                    if (Input(btnName) && AutoFind)
                    {
                        OnFindRoom();
                    }
                    break;
            }
        }

        public void OnClickWithInput()
        {
            var roomId = GetCurRoomId();
            if (roomId.Length < MaxIdCount) return;
            OnFindRoom();
        }

        private void OnFindRoom()
        {
            int roomType;
            var roomId = GetCurRoomId();
            if (!int.TryParse(roomId, out roomType)) return;
            Dictionary<string, object> dic = new Dictionary<string, object>();
            dic["id"] = roomId;
            Facade.Instance<TwManager>().SendAction("group.teaGetIn", dic, GetInTea);
        }
     
        private void GetInTea(object msg)
        {
            Dictionary<string, object> dic = (Dictionary<string, object>) msg;
            long value = (long)dic["mstatus"];
            if (value != 4)
            {
                YxWindow obj = CreateOtherWindow("TeaPanel");
                TeaPanel panel = obj.GetComponent<TeaPanel>();
                panel.UpdateView(dic);
                panel.SetTeaCode(int.Parse(GetCurRoomId()));
                Util.SetString(KeyTeaId, GetCurRoomId());
                Close();
            }
            else
            {
                YxMessageBox.Show(string.Format("{0}不存在", CurrentName));
            }
            Clear();
        }

        public void OnOpenCreateWindow()
        {
            var win = YxWindowManager.OpenWindow("CreateRoomWindow", true);
            var createWin = (CreateRoomWindow)win;
            if (createWin == null) return;
            createWin.TabDefaultIndex = TabDefaultIndex;
        }
        private int _curInputIndex;
        /// <summary>
        /// 清空操作
        /// </summary>
        private void Clear()
        {
            if (RoomIdMoreLabel.Count > 0)
            {
                for (var i = 0; i < MaxIdCount; i++)
                {
                    var label = RoomIdMoreLabel[i];
                    label.text = "";
                    _curInputIndex = 0;
                }
                return;
            }
            RoomIdLabel.text = "";
        }
        /// <summary>
        /// 删除操作
        /// </summary>
        private void Delete()
        {
            if (RoomIdMoreLabel.Count > 0)
            {
                if (_curInputIndex > 0)
                {
                    _curInputIndex--;
                    var label = RoomIdMoreLabel[_curInputIndex];
                    label.text = "";
                }
                return;
            }
            var cur = RoomIdLabel.text;
            if (cur.Length < 1) return;
            RoomIdLabel.text = cur.Remove(cur.Length - 1);
        }
        /// <summary>
        /// 输入数据处理
        /// </summary>
        /// <param name="num"></param>
        /// <returns></returns>
        private bool Input(string num)
        {
            if (RoomIdMoreLabel.Count > 0)
            {
                if (_curInputIndex >= MaxIdCount) return true;
                var label = RoomIdMoreLabel[_curInputIndex];
                label.text = num;
                _curInputIndex++;
                return _curInputIndex >= MaxIdCount;
            }
            if (RoomIdLabel.text.Length >= MaxIdCount) return true;
            var roomId = string.Format("{0}{1}", RoomIdLabel.text, num);
            RoomIdLabel.text = roomId;
            return RoomIdLabel.text.Length >= MaxIdCount;
        }
        /// <summary>
        /// 处理RoomId
        /// </summary>
        /// <returns></returns>
        private string GetCurRoomId()
        {
            if (RoomIdMoreLabel.Count > 0)
            {
                var roomId = "";
                for (var i = 0; i < MaxIdCount; i++)
                {
                    roomId = string.Format("{0}{1}", roomId, RoomIdMoreLabel[i].text);
                }
                return roomId;
            }
            return RoomIdLabel.text;
        }
    }
}
