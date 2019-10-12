﻿using System;
using Assets.Scripts.Common.Adapters;
using Assets.Scripts.Common.Components;
using Assets.Scripts.Hall.View.AboutRoomWindows;
using com.yxixia.utile.YxDebug;
using UnityEngine;
using YxFramwork.Common;
using YxFramwork.Common.Model;
using YxFramwork.Controller;
using YxFramwork.Manager;
using YxFramwork.View;

namespace Assets.Scripts.Hall.View
{
    [RequireComponent(typeof(NguiPanelAdapter))]
    [RequireComponent(typeof(Rigidbody))]
    [Obsolete("Use Assets.Scripts.Hall.View.ListViews.GameListItem")]
    public class GameListItem : NguiListItem
    {
        [Tooltip("背景")]
        public UISprite Background;
        [Tooltip("背景名称前缀，后边会自动加上游戏列表分组对应的值")]
        public string BackgroundNamePrefix;
        [Tooltip("游戏名称")]
        public NguiLabelAdapter GameNameLabel;
        [Tooltip("Item默认的box大小，如果GamelistItemView中的Widget为空时，将使用此Widget")]
        public UIWidget DefaultBoxWidget;
        private UIButton _btn;
        private GameListItemView _itemView;
        private NguiPanelAdapter _panelAdapter;
        private GameUnitModel _model;
        
        protected override void OnAwake()
        {
            _panelAdapter = GetComponent<NguiPanelAdapter>();
            _btn = GetComponent<UIButton>();
//            _btn.state = UIButtonColor.State.Disabled;
        }

        protected override void OnChangeData(IItemData itemData, string itemType)
        {
            _model = itemData as GameUnitModel;
            if (_model == null)
            {
                gameObject.SetActive(false);
                return;
            }
            gameObject.SetActive(true);
            if (GameNameLabel != null) GameNameLabel.Text(_model.GameName);
            if (Background != null)
            {
                var gm = GameListModel.Instance;
                var group = gm.GetGroup(gm.CurGroup);
                Background.spriteName = string.Format("{0}{1}", BackgroundNamePrefix, group.Type);
            } 
            name = _model.GameKey;
            if (_itemView != null) { Destroy(_itemView.gameObject); }
            var assetname = string.Format("gamelist_{0}",name);
            if (!string.IsNullOrEmpty(itemType))
            {
                assetname = string.Format("{0}_{1}", assetname, itemType);
            }
            var bundlePrefix = string.Format("{0}_{1}", App.Skin.GameInfo,name);
            var bundleName = string.Format("{0}/{1}", bundlePrefix, assetname);
            var go = ResourceManager.LoadAsset(App.Skin.GameInfo, bundleName, assetname);//App.HallName
            if (go == null) return;
            go = Instantiate(go);
            var ts = go.transform;
            var lcScale = ts.localScale;
            var lcPos = ts.localPosition;
            var lcRot = ts.localRotation;
            ts.parent = transform;
            ts.localPosition = lcPos;
            ts.localRotation = lcRot;
            ts.localScale = lcScale;
            _itemView = go.GetComponent<GameListItemView>();
            if (_itemView!=null)
            {
                _itemView.FreshBtnClickBound(_btn, DefaultBoxWidget, _model.GameState == GameState.Developing);
            }
            else
            {
                YxDebug.LogError("没有GameListItemView","GameListItem");
            }
        }

        /// <summary>
        /// 点击游戏列表，进入房间列表
        /// </summary>
        public void OnGameClick()
        {
            if (_model == null) return;
            switch (_model.GameState)
            {
                case GameState.Developing:
                    YxMessageBox.Show("游戏在努力开发中，敬请期待!!");
                    return;
                case GameState.CreateMode:
                    OnOpenCreateWindow();
                    break;
                case GameState.Match:
                    OnOpenGame();
                    break; 
                default:
                    RoomListController.Instance.GetRoomlistAndShow(_model);
                    break;
            }
        }
         
        /// <summary>
        /// 点击游戏列表，打开创建窗口
        /// </summary>
        public void OnOpenCreateWindow()
        {
            if (_model != null && _model.GameState == GameState.Developing)
            {
                YxMessageBox.Show("游戏在努力开发中，\r\n敬请期待!!!!");
                return;
            }
            var win = CreateOhterWindowWithT<CreateRoomWindow>("CreateRoomWindow");
            win.GameKey = name;
        }

        /// <summary>
        /// 点击游戏列表，打开创建窗口//todo 临时处理
        /// </summary>
        public void OnOpenAssuredCreateWindow()
        {
            if (_model != null && _model.GameState == GameState.Developing)
            {
                YxMessageBox.Show("游戏在努力开发中，\r\n敬请期待!!!!");
                return;
            }
            var win = CreateOhterWindowWithT<CreateRoomWindow>("CreateRoomWindow");
            win.GameKey = name;
            win.IsDesignated = true;
        }

        /// <summary>
        /// 点击游戏列表，打开游戏
        /// </summary>
        public void OnOpenGame()
        {
            if (_model == null) return;
            RoomListController.Instance.QuickGame(_model.GameKey);
        }

        /// <summary>
        /// 激活
        /// </summary>
        /// <param name="isAction"></param>
        public override void AwakAction(bool isAction)
        {
            if (_itemView == null) return;
            _itemView.AwakAction(isAction);
            if (isAction) _itemView.SetOrder(Order);
        }

        /// <summary>
        /// 颜色
        /// </summary>
        /// <param name="color"></param>
        public override void SetColor(Color color)
        {
            if (_btn == null) return;
            _btn.defaultColor = color;
            if (_itemView == null) return;
            _itemView.SetColor(color);
        }

        public override void SetOrder(int order)
        {
            Order = order; 
            _panelAdapter.SortingOrder(order);
        }
    }
}
