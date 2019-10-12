using Assets.Scripts.Common.Utils;
using Assets.Scripts.Hall.View.PageListWindow;
using com.yxixia.utile.Utiles;
using com.yxixia.utile.YxDebug;
using System;
using System.Collections.Generic;
using UnityEngine;
using YxFramwork.Framework;

namespace Assets.Scripts.Hall.View.RecordWindows
{
    public class DbsRecordDetialWindow : YxPageListWindow
    {
        public string[] NeedShowPokerGameKeys;
        [Tooltip("pokerItemԤ��")]
        public DbsRecordDetialItemView PokerItem;

        #region Data Param
        /// <summary>
        ///key ��Ϸkey
        /// </summary>
        private string _keyGameKey = "game_key_c";
        /// <summary>
        /// key��ʵ�����
        /// </summary>
        private string _keyRoomId = "room_id";
        #endregion 
        #region Local Data
        /// <summary>
        ///  ��ǰ��Ϸgamekey
        /// </summary>
        private string _curGamekey;
        /// <summary>
        ///  ��ǰ��¼roomId
        /// </summary>
        private string _curRoomId;
        /// <summary>
        /// ��ʱ����
        /// </summary>
        private RecordDetailData _curData;
        /// <summary>
        /// �Ƿ�Ϊ�����һ�δ�
        /// </summary>
        private bool _isFirst = true;

        #endregion

        protected override Type GetItemType()
        {
            return typeof(RecordDetialItemData);
        }

        public void SendFirstAction(string gamekey, string roomId, bool playBack)
        {
            _curRoomId = roomId;
            _curGamekey = gamekey;
            DbsRecordDetialItemView.PlayBack = playBack;
            RecordDetailItem.PlayBack = playBack;
            Show();
            FirstRequest();
        }

        protected override void SetActionDic()
        {
            base.SetActionDic();
            ActionParam[_keyGameKey] = _curGamekey;
            ActionParam[_keyRoomId] = _curRoomId;
        }

        protected override void OnActionCallBackDic()
        {
            _curData = new RecordDetailData(Data, GetItemType());
            RecordDetailItem.Host = _curData.WebHost;
            DealPageData(_curData);
        }

        protected override void RefreshView(List<YxData> data, int startIndex = 0)
        {
            for (int i = startIndex, endIndex = data.Count + startIndex; i < endIndex; i++)
            {
                YxView view;
                if (IsNeedShowPoker())
                {
                    view = Table.transform.GetChildView(i, PokerItem);
                }
                else
                {
                    view = Table.transform.GetChildView(i, ListItem);
                }
                var itemData = data[i - startIndex];
                view.Id = (IdAntitone ? _totalCount - i : i + 1).ToString();
                view.UpdateView(itemData);
            }
            Table.repositionNow = true;
        }

        private bool IsNeedShowPoker()
        {
            if (NeedShowPokerGameKeys == null) return false;
            for (int i = 0; i < NeedShowPokerGameKeys.Length; i++)
            {
                if (_curGamekey == NeedShowPokerGameKeys[i])
                    return true;
            }
            return false;
        }

        /// <summary>
        /// ʹ����Ƕ��ҳ�򿪻ط�
        /// </summary>
        /// <param name="openWindowName"></param>
        /// <param name="url"></param>
        public void ShowUrlByWebView(string openWindowName, string url)
        {
            MainYxView.OpenWindowWithData(openWindowName, url);
        }

        public void OpenUrl(string url)
        {
            YxDebug.Log(string.Format("Url is: {0}", url));
            Application.OpenURL(url);
        }

        protected override void HideTableItems()
        {
            var list = Table.GetChildList();
            foreach (var item in list)
            {
                Destroy(item.gameObject);
            }
        }
    }
}