﻿using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Hall.View.AboutRoomWindows;
using com.yxixia.utile.Utiles;
using UnityEngine;
using YxFramwork.Common;
using YxFramwork.Common.Model;
using YxFramwork.Controller;
using YxFramwork.Enums;
using YxFramwork.Framework;
using YxFramwork.Manager;
using YxFramwork.View.YxListViews;

namespace Assets.Scripts.Hall.View.HallWindows
{
    /// <summary>
    /// 房间列表视图
    /// </summary>
    public class RoomListWindow : YxBaseWindow
    { 
        [Tooltip("房间列表")]
        public YxListView ListView;
        [Tooltip("创建房间视图")]
        public GameObject CreateRoomView;
        [Tooltip("休闲视图")]
        public GameObject FreeRoomView;
        [Tooltip("指定gamekey")]
        public string AppointGameKey;
        [Tooltip("MainWindow需要隐藏的内容")]
        public GameObject[] HideUIs;
        [Tooltip("游戏名称label")]
        public UILabel GameNameLabel;
        [Tooltip("需要加载的资源，gameobject的名字为资源名称，加载的内容会在gameobject中创建")]
        public GameObject[] LoadObjects;
        private readonly Dictionary<string,GameObject> _loadedObject = new Dictionary<string, GameObject>();
        private int _state;

        protected override void OnAwake()
        {
            base.OnAwake();
            ChangeState(); 
        }

        public override YxEWindowName WindowName
        {
            get { return YxEWindowName.RoomList;}
        }

        protected override IBaseModel YxBaseModel
        {
            get { return RoomListModel.Instance; }
        }
           
        protected override void OnShow(object o)
        { 
            base.OnShow(o);
            if (!string.IsNullOrEmpty(AppointGameKey))
            {
                App.LoadingGameKey = AppointGameKey;
                RoomListController.Instance.GetRoomList(AppointGameKey);
            } 
        }

        /// <summary>
        /// 刷新游戏名称
        /// </summary>
        public void FreshGameName()
        {
            if (GameNameLabel == null) return;
            var gameModels = GameListModel.Instance.GameUnitModels;
            var gamekey = App.LoadingGameKey;
            if (!gameModels.ContainsKey(gamekey)) return;
            var gameModel = gameModels[gamekey];
            if (gameModel != null)
            {
                GameNameLabel.text = gameModel.GameName;
            }
        }

        /// <inheritdoc />
        /// <summary>
        /// 绑定数据刷新
        /// </summary>
        /// <param name="isChange"></param>
        protected override void OnBindDate(bool isChange = false)
        {
            base.OnBindDate(isChange);
            var model = RoomListModel.Instance; 
            if (model.RoomKind > 0)
            {
                ShowCreateRoom();
            }
            else
            { 
                ShowFreeRoom();
            }
            CurtainManager.CloseCurtain();
            FreshGameName();
            LoadOtherObject();
        }

        /// <summary>
        /// 加载其他物体
        /// </summary>
        private void LoadOtherObject()
        {
            DestroyAllLoadedObject();
            var objectCount = LoadObjects.Length;
            var gamekey = App.LoadingGameKey;
            var prefix = App.Skin.GameInfo;
            for (var i = 0; i < objectCount; i++)
            {
                var loadObject = LoadObjects[i];
                var loadItemName = loadObject.name;
                var namePrefix = string.Format("{0}_{1}", prefix, gamekey);
                var bundleName = string.Format("{0}/{1}", namePrefix, loadItemName);
                var prefabObj = ResourceManager.LoadAsset(prefix, bundleName, loadItemName);
                var go = GameObjectUtile.Instantiate(prefabObj, loadObject.transform);
                _loadedObject[loadItemName] = go;
            }
        }

        /// <summary>
        /// 移除所有另外加载的对象
        /// </summary>
        private void DestroyAllLoadedObject()
        {
            var arr = _loadedObject.Keys.ToArray();
            var len = arr.Length;
            for (var i = 0; i < len; i++)
            {
                var key = arr[i];
                if(!_loadedObject.ContainsKey(key)) { continue;}
                Destroy(_loadedObject[key]);
            }
            _loadedObject.Clear();
        }

        /// <summary>
        /// 显示房间列表
        /// </summary>
        private void ShowFreeRoom()
        {
            ChangeState(1);
            var model = RoomListModel.Instance; 
            var list = model.RoomUnitModel;
            ListView.UpdateView(list);
        }

        /// <summary>
        /// 显示创建房间界面
        /// </summary>
        public void ShowCreateRoom()
        {
            ChangeState(2);
        }

        public void ChangeState(int state=0)
        {
            _state = state;
            if (CreateRoomView != null) CreateRoomView.SetActive(_state == 2);
            else _state = 1;
            if (FreeRoomView != null) FreeRoomView.gameObject.SetActive(_state == 1);
        }

        /// <summary>
        /// 点击free图标
        /// </summary>
        public void OnFreeRoomClick()
        { 
            ShowFreeRoom();
        }

        /// <summary>
        /// 打开创建房间窗口
        /// </summary>
        /// <param name="index"></param>
        public void OnOpenCreateWindow(string index)
        {
            var win = YxWindowManager.OpenWindow("CreateRoomWindow");
            var createWin = (CreateRoomWindow)win;
            if (createWin == null) return;
            int dindex;
            int.TryParse(index,out dindex);
            createWin.TabDefaultIndex = dindex;
        }

        /// <summary>
        /// 通过GameKey打开创建房间窗口
        /// </summary>
        public void OnOpenCreateWindowWithKey()
        { 
            var win = YxWindowManager.OpenWindow("CreateRoomWindow");
            if (win == null) return;
            var cwin = win.GetComponent<CreateRoomWindow>();
            if (cwin == null) return;
            cwin.GameKey = App.LoadingGameKey;
        }

        /// <summary>
        /// 打开查找放房间
        /// </summary>
        /// <param name="index"></param>
        public void OnFindRoomWindow(string index)
        {
            var win = YxWindowManager.OpenWindow("FindRoomWindow");
            var findWin = (FindRoomWindow)win;
            if (findWin == null) return;
            int dindex;
            int.TryParse(index, out dindex);
            findWin.TabDefaultIndex = dindex;
        }

        protected override void OnEnable()
        {
            YxWindowUtils.DisplayUI(HideUIs, false);
        }

        protected override void OnDisable()
        {
            YxWindowUtils.DisplayUI(HideUIs);
        }

        protected override void OnClose()
        {
            base.OnClose();
            if (RoomListModel.Instance.RoomKind > 0 && _state == 1) ChangeState(2);
            else
            {
                if (App.History.LastHistory() == YxEHistoryPathType.Hall)
                {
                    CurPanelManager.ShowWindow(YxEWindowName.GameHall);
                }
                else
                {
                    GameListController.Instance.ShowGameList(GameListModel.Instance.CurGroup);
                } 
            }
//            else HallMainController.Instance.ReturnHall();
        }
         
        /// <summary>
        /// 返回大厅主页面
        /// </summary>
        public void ReturnMain()
        {
            HallMainController.Instance.ShowHallMain();
        }


        public void OnQuicklyGame()
        {
            GameListController.Instance.QuickGame(App.LoadingGameKey);
        }
    }
}

