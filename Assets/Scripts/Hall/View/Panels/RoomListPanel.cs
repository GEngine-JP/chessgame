using System.Collections;
using com.yxixia.utile.Utiles;
using UnityEngine;
using YxFramwork.Common;
using YxFramwork.Common.Adapters;
using YxFramwork.Common.Model;
using YxFramwork.Controller;
using YxFramwork.Manager;
using YxFramwork.View;
using YxListView = YxFramwork.View.YxListViews.YxListView;

namespace Assets.Scripts.Hall.View.Panels
{
    /// <summary>
    /// / �����б���� ��TODO �Ժ�ͳһ�������
    /// </summary>
    public class RoomListPanel : YxBasePanel
    {
        /// <summary>
        /// ��������
        /// </summary>
        [Tooltip("��������")]
        public YxBaseLabelAdapter RoomName;
        /// <summary>
        /// ��Ϸ����label
        /// </summary>
        [Tooltip("��Ϸ����label")]
        public YxBaseLabelAdapter GameNameLabel;
        [Tooltip("ָ��gamekey")]
        public string AppointGameKey;
        [Tooltip("�����б�")]
        public YxListView DefaultListView;
        /// <summary>
        /// ��ǰlistView
        /// </summary>
        protected YxListView CurListView;

        protected override void OnAwake()
        {
            base.OnAwake();
            NeedFreshWithShow = true;
            DefaultListView.SetActive(false);
        }

        protected override void OnFreshView()
        {
            base.OnFreshView();
            var gk = App.LoadingGameKey;
            if (string.IsNullOrEmpty(gk)) { return; }
            if (gk == _gameKey)
            {
                if (CurListView != null)
                {
                    CurListView.Show();
                }
                return;
            }
            DestroyLastExView();
            CurListView = CreateListView(gk);
            OnShowAllListItem(gk);
            ShowViewEx(gk);
            FreshGameName(gk);
            _gameKey = gk;
        }

        protected void DestroyLastExView()
        {
            if (ExView != null)
            {
                Destroy(ExView.gameObject);
            }
        }

        protected Transform ExView;
        /// <summary>
        /// ������չview
        /// </summary>
        /// <param name="gk"></param>
        protected virtual void ShowViewEx(string gk)
        {
            var w = GameObjectUtile.CreateMonoContainer<UIWidget>(transform);
            ExView = w.transform;
            var prefix = App.Skin.GameInfo;
            var namePrefix = string.Format("{0}_{1}", prefix, gk);//gameinfo_gamekey
            var viewExName = string.Format("roomlistviewex_{0}", gk);//roomlistview_gamekey
            var bundleName = string.Format("{0}/{1}", namePrefix, viewExName);//gameinfo_gamekey/roomlistview_gamekey
            var pre = ResourceManager.LoadAsset(prefix, bundleName, viewExName);
            if (pre == null) { return; }
            pre = Instantiate(pre);
            GameObjectUtile.ResetTransformInfo(pre.transform,ExView.transform);
        }

        protected virtual IList GetListData()
        {
            var model = RoomListModel.Instance;
            return model.RoomUnitModel;
        }

        private void OnShowAllListItem(string gk)
        {
            if (CurListView == null) return;
            var list = GetListData();
            CurListView.Init();
            CurListView.UpdateView(list);
            CurListView.Show();
//            Util.SetInt("RoomList_StartDataIndex", CurListView.StartDataIndex);
        }

        private string _gameKey;
        /// <summary>
        /// �����б���ͼ
        /// </summary>
        private YxListView CreateListView(string gk)
        {
            if (CurListView != null)
            { 
                if (CurListView == DefaultListView)
                {
                    DefaultListView.SetActive(false);
                }
                else
                {
                    Destroy(CurListView.gameObject);
                }
            }
            var pre = LoadListViewResource(gk);
            if (pre == null) { return DefaultListView; }
            var go = Instantiate(pre);
            var listView = go.GetComponent<YxListView>();
            return listView == null ? DefaultListView : listView;
        }

        /// <summary>
        /// ������Դ
        /// </summary>
        /// <returns></returns>
        protected virtual GameObject LoadListViewResource(string gk)
        {
            var prefix = App.Skin.GameInfo;
            var namePrefix = string.Format("{0}_{1}", prefix, gk);//gameinfo_gamekey
            var listName = string.Format("roomlistview_{0}", gk);//roomlistview_gamekey
            var bundleName = string.Format("{0}/{1}", namePrefix, listName);//gameinfo_gamekey/roomlistview_gamekey
            return ResourceManager.LoadAsset(prefix, bundleName, listName);
        }

        /// <summary>
        /// ˢ����Ϸ����
        /// </summary>
        public void FreshGameName(string gk)
        {
            if (GameNameLabel == null) return;
            var gameModels = GameListModel.Instance.GameUnitModels;
            if (!gameModels.ContainsKey(gk)) return;
            var gameModel = gameModels[gk];
            if (gameModel != null)
            {
                GameNameLabel.Text(gameModel.GameName);
            }
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            if (CurListView != null)
            {
                CurListView.Hide();
            }
        }


        public void OnQuicklyGame()
        {
            GameListController.Instance.QuickGame(App.LoadingGameKey);
        }
    }
}
