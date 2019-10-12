using Assets.Scripts.Common.Windows.TabPages;
using System.Collections.Generic;
using com.yxixia.utile.Utiles;
using UnityEngine;
using Assets.Scripts.Hall.View.NnRecordWindow;
using YxFramwork.Common;
using Assets.Scripts.Common.Utils;
using System;

namespace Assets.Scripts.Hall.View.RecordWindows
{
    public class DbsRecordDetialItemView : YxTabItem
    {
        [Tooltip("ս������Grid")]
        public UIGrid DetialGrid;
        [Tooltip("ս������Ԥ��")]
        public DbsRecordDetialItem DetialItem;
        [Tooltip("����ͼ")]
        public UISprite BgSprite;
        [Tooltip("��սʱ��")]
        public UILabel Time;
        [Tooltip("��¼ID")]
        public UILabel RoundNum;
        [Tooltip("�طſ��ذ�ť")]
        public GameObject ReplayBtnParent;
        [Tooltip("�����ֱ�־")]
        public GameObject InterlacedBgSprite;

        public string RoundFormat = "��{0}��";
        /// <summary>
        /// �طſ���
        /// </summary>
        public static bool PlayBack;
        /// <summary>
        /// host��detailItem����
        /// </summary>
        public static string Host;

        private UIGrid _detialGrid;
        private readonly List<UserData> _userDatas = new List<UserData>();
        #region Local Data
        /// <summary>
        /// ��������
        /// </summary>
        private RecordDetialItemData _curData;
        #endregion

        protected override void OnFreshView()
        {
            CheckItemData();
        }

        protected void CheckItemData()
        {
            var data = Data as RecordDetialItemData;
            if (data == null) return;
            _curData = data;
            ShowItem(Id, App.UserId);
        }

        public string ReplayUrl
        {
            get { return string.Format("{0}{1}", Host, _curData.Url); }
        }

        public void ShowItem(string roundNum, string selfId)
        {
            _curData.ShowRoundNum = roundNum;
            SetItemInfo(_curData);
        }

        /// <summary>
        /// ����ʱ�䣬���������Ϣ
        /// </summary>
        /// <param name="data"></param>
        private void SetItemInfo(RecordDetialItemData data)
        {
            Time.TrySetComponentValue(data.Time);
            var roundNumString = data.ShowRoundNum;
            RoundNum.TrySetComponentValue(string.Format(RoundFormat, roundNumString));
            ReplayBtnParent.TrySetComponentValue(PlayBack);
            ReplayBtnParent.TrySetComponentValue(PlayBack);
            var heads = data.HeadDatas;
            int index = 0;
            foreach (var item in heads)
            {
                var view = DetialGrid.transform.GetChildView(index++, DetialItem);
                view.UpdateView(item.Value);
            }
            DetialGrid.repositionNow = true;
            //����������ʾ
            SetInterlacedBg(roundNumString);
        }

        /// <summary>
        /// ���ý���������ʾ
        /// </summary>
        /// <param name="roundNumStr"></param>
        public void SetInterlacedBg(string roundNumStr)
        {
            if (InterlacedBgSprite == null) return;
            int roundNum;
            if (!int.TryParse(roundNumStr, out roundNum)) return;
            InterlacedBgSprite.SetActive(roundNum % 2 != 0);
        }

        protected Type GetItemType()
        {
            return typeof(RecordDetialItemData);
        }
        protected override void OnHide()
        {
            var list = DetialGrid.GetChildList();
            foreach (var item in list)
            {
                item.gameObject.SetActive(false);
            }
        }

        public override void Hide()
        {
            OnHide();
            gameObject.SetActive(false);
        }
    }
}