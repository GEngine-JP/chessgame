﻿using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Assets.Scripts.Game.sss.ImgPress.Main;
using Assets.Scripts.Game.sss.Tool;
using com.yxixia.utile.YxDebug;
using Sfs2X.Entities.Data;
using Sfs2X.Requests;
using UnityEngine;
using YxFramwork.Common;
using YxFramwork.Framework.Core;
using YxFramwork.Manager;
using YxFramwork.View;

// ReSharper disable FieldCanBeMadeReadOnly.Local
#pragma warning disable 649

namespace Assets.Scripts.Game.sss
{
    public class SettingMenu : MonoBehaviour
    {

        [Tooltip("设置窗口对象")]
        [SerializeField]
        private GameObject _settingView = null;

        public GameObject HistoryView = null;

        [Tooltip("音乐音量设置")]
        [SerializeField]
        private UISlider _volumeSlider = null;

        [Tooltip("音效音量设置")]
        [SerializeField]
        private UISlider _effectSlider = null;

        protected void Start()
        {
            InitVolumeVal();
            InitChoiseModel();
        }



        /// <summary>
        /// 向上箭头图标
        /// </summary>
        [SerializeField]
        private GameObject _sprUp;


        /// <summary>
        /// 向下箭头图标
        /// </summary>
        [SerializeField]
        private GameObject _sprDown;

        /// <summary>
        /// 关闭下拉菜单按钮
        /// </summary>
        [SerializeField]
        private GameObject _closeListBtn;

        /// <summary>
        /// 更换房间按钮
        /// </summary>
        [SerializeField]
        private GameObject _historyBtn;

        /// <summary>
        /// 退出按钮
        /// </summary>
        [SerializeField]
        private UIButton _quitBtn;

        /// <summary>
        /// 特殊按钮
        /// </summary>
        [SerializeField]
        private UIButton _changeRoomBtn;

        /// <summary>
        /// 打开下拉菜单按钮
        /// </summary>
        [SerializeField]
        private BoxCollider _openListBtn;


        /// <summary>
        /// 下拉菜单的TweenScale
        /// </summary>
        [SerializeField]
        private TweenPosition _dropDownListTp;

        /// <summary>
        /// 游戏规则界面
        /// </summary>
        [SerializeField]
        private GameObject _helpView = null;

        /// <summary>
        /// 所有菜单中，包含Tween动画的按钮
        /// </summary>
        [SerializeField]
        private List<GameObject> _tweenList;


        /// <summary>
        /// 选牌模式的Toggle
        /// </summary>
        [SerializeField]
        private UIToggle[] _toggles;

        private const string ChoiseModelKey = "ChoiseModel";



        /// <summary>
        /// 所有按钮
        /// </summary>
        [SerializeField]
        // ReSharper disable once CollectionNeverUpdated.Local
        private List<GameObject> _buttons = new List<GameObject>();

        private string _dismissBtnName = "dismissroom";


        public void InitVolumeVal()
        {
            _settingView.gameObject.SetActive(false);
            _volumeSlider.value = Facade.Instance<MusicManager>().MusicVolume;
            _effectSlider.value = Facade.Instance<MusicManager>().EffectVolume;
            InitOnClick();
        }


        private void InitChoiseModel()
        {
            int index = PlayerPrefs.HasKey(ChoiseModelKey) ? PlayerPrefs.GetInt(ChoiseModelKey) : 0;
            SetModel(index);
            _toggles[index].Set(true);
        }


        /// <summary>
        /// 设置开放模式菜单
        /// </summary>
        public void SetRoomModelList()
        {
            var gdata = App.GetGameData<SssGameData>();
            //不是开放模式不需要更换图片
            if (!gdata.IsRoomGame)
                return;

            bool daikai = gdata.DaiKai;
            bool isOwner = App.GetGameManager<SssGameManager>().IsRoomOwner;

            if (gdata.IsPlayed || (isOwner && !daikai))     //游戏已经开始,取消解散房间按钮改为投票解散房间按钮
            {
                _quitBtn.disabledSprite = _dismissBtnName + "_up";
                _quitBtn.normalSprite = _dismissBtnName + "_up";
                _quitBtn.pressedSprite = _dismissBtnName + "_over";
                _quitBtn.hoverSprite = _dismissBtnName + "_over";

                Tools.NguiAddOnClick(_quitBtn.gameObject, OnClickListener, (int)MenuBtns.DismissRoom);
                _changeRoomBtn.gameObject.SetActive(false);
                _historyBtn.GetComponent<TweenScale>().onFinished = null;

            }
            else
            {
                //如果是代开模式,将更换房间变为解散房间
                if (isOwner)
                {
                    _changeRoomBtn.disabledSprite = _dismissBtnName + "_up";
                    _changeRoomBtn.normalSprite = _dismissBtnName + "_up";
                    _changeRoomBtn.pressedSprite = _dismissBtnName + "_over";
                    _changeRoomBtn.hoverSprite = _dismissBtnName + "_over";
                    _changeRoomBtn.onClick = new List<EventDelegate> { new EventDelegate(OnClickDissolveBtn) };
                }
                else
                {
                    //开放模式下,隐藏更换房间按钮
                    _tweenList.Remove(_changeRoomBtn.gameObject);
                }
            }
        }


        public void OnDragVolumeSlider(float volume)
        {

            Facade.Instance<MusicManager>().MusicVolume = volume;

        }

        public void OnDragEffectSlider(float volume)
        {

            Facade.Instance<MusicManager>().EffectVolume = volume;

        }

        public void OnClickChangeRoomBtn()
        {
            CloseListView();
            var gdata = App.GetGameData<SssGameData>();
            if (gdata.IsPlaying)
            {
                YxDebug.Log("正在游戏中,无法更换房间!");
                YxMessageBox.Show(new YxMessageBoxData
                {
                    Msg = "正在游戏中,无法更换房间!",
                    Delayed = 5,
                });
            }
            else
            {
                YxDebug.Log("正在更换房间....");
                App.GetRServer<SssGameServer>().ChangeRoom();
            }
        }


        /// <summary>
        /// 打开设置窗口
        /// </summary>
        public void OnClickSettingBtn()
        {
            CloseListView();
            _settingView.gameObject.SetActive(true);
        }


        public void OnClickHistoryBtn()
        {
            YxDebug.Log("打开查看历史战绩界面");
            CloseListView();
            HistoryView.SetActive(true);
        }


        /// <summary>
        /// 关闭设置窗口
        /// </summary>
        public void OnClickCloseSettingViewBtn()
        {
            YxDebug.Log("Click Close Setting View");
            _settingView.SetActive(false);
        }


        /// <summary>
        /// 打开下拉菜单
        /// </summary>
        public void OnClickOpenListBtn()
        {
            _dropDownListTp.PlayForward();
            _sprUp.SetActive(true);
            _sprDown.SetActive(false);
            _closeListBtn.SetActive(true);
            _openListBtn.enabled = false;


            var preTweenScale = _tweenList[0];

            for (int i = 1; i < _tweenList.Count; i++)
            {
                var nextTween = _tweenList[i];
                if (!nextTween.activeSelf)
                    continue;

                SetTween<TweenScale>(preTweenScale, nextTween);
                SetTween<TweenColor>(preTweenScale, nextTween);
                preTweenScale = nextTween;
            }
        }

        private void SetTween<T>(GameObject preTween, GameObject nextTween) where T : UITweener
        {
            var preT = preTween.GetComponent<T>();
            var nextT = nextTween.GetComponent<T>();

            var eds = new EventDelegate(nextT, "PlayForward");
            preT.onFinished.Clear();
            preT.onFinished.Add(eds);
            nextT.ResetToBeginning();
        }


        /// <summary>
        /// 房主解散房间
        /// </summary>
        public void OnClickDissolveBtn()
        {
            if (App.GetGameManager<SssGameManager>().IsRoomOwner && App.GetGameData<SssGameData>().DaiKai)
                DissolveRoomWithMessageBox();
        }

        public void OnClickCloseListBtn()
        {
            CloseListView();
        }

        /// <summary>
        /// 关闭下拉菜单
        /// </summary>
        void CloseListView()
        {
            _dropDownListTp.PlayReverse();
            _sprDown.SetActive(true);
            _sprUp.SetActive(false);
            _closeListBtn.SetActive(false);
            _openListBtn.enabled = true;

            foreach (GameObject tween in _tweenList)
            {
                var ts = tween.GetComponent<TweenScale>();
                if (ts != null)
                {
                    ts.ResetToBeginning();
                }
                var tc = tween.GetComponent<TweenColor>();
                if (tc != null)
                {
                    tc.ResetToBeginning();
                }
            }
        }

        public void DismissRoom()
        {
            CloseListView();

            App.GetGameManager<SssGameManager>().DismissRoomMgr.SetDismissRoomBtn();
            //YxMessageBox.Show(new YxMessageBoxData
            //{
            //    Msg = "解散房间方法正在完善中,请等待更新!",
            //    Delayed = 5,
            //});
            //YxDebug.Log("点击了解散房间按钮");
        }

        public void OnClickRuleBtn()
        {
            CloseListView();
            _helpView.SetActive(true);
        }

        /// <summary>
        /// 退出游戏
        /// </summary>
        public void QuitGame()
        {
            CloseListView();
            var gdata = App.GetGameData<SssGameData>();
            if (gdata.IsPlaying)
            {
                YxDebug.Log("正在游戏中,无法退出游戏!");
                YxMessageBox.Show(new YxMessageBoxData
                {
                    Msg = "正在游戏中,无法退出游戏!",
                    Delayed = 5,
                });
            }
            else
            {
                YxDebug.Log("正在退出游戏...");
                bool daikai = gdata.DaiKai;
                bool isMine = App.GetGameManager<SssGameManager>().IsRoomOwner;

                if (!daikai && isMine)
                {
                    DissolveRoomWithMessageBox();
                }
                else
                {
                    QuitWithMessageBox();
                }
            }
        }


        public void QuitWithMessageBox()
        {
            YxMessageBox.Show(new YxMessageBoxData
            {
                Msg = "您确定要退出游戏吗!?",
                IsTopShow = true,
                BtnStyle = YxMessageBox.LeftBtnStyle | YxMessageBox.RightBtnStyle,
                Listener = (box, btnName) =>
                 {
                     if (btnName == YxMessageBox.BtnLeft)
                     {
                         App.QuitGame();
                     }
                 }
            });
        }

        public void DissolveRoomWithMessageBox()
        {
            YxMessageBox.Show(new YxMessageBoxData
            {
                Msg = "您确定要解散本房间吗!?",
                IsTopShow = true,
                BtnStyle = YxMessageBox.LeftBtnStyle | YxMessageBox.RightBtnStyle,
                Listener = (box, btnName) =>
                {
                    if (btnName == YxMessageBox.BtnLeft)
                    {
                        IRequest req = new ExtensionRequest("dissolve", new SFSObject());
                        App.GetRServer<SssGameServer>().SendRequest(req);
                    }
                }
            });
        }


        ///<summary>
        ///初始化按键监听
        ///</summary>
        public void InitOnClick()
        {
            foreach (MenuBtns btnid in Enum.GetValues(typeof(MenuBtns)))
            {
                foreach (GameObject btn in _buttons)
                {
                    if (btn.name.Equals(btnid.ToString()))
                    {
                        Tools.NguiAddOnClick(btn, OnClickListener, (int)btnid);
                    }
                }
            }
        }

        public void OnCLickChoiseModelBtn(string objName)
        {
            int model = int.Parse(objName);
            SetModel(model);
        }

        void SetModel(int model)
        {
            var gdata = App.GetGameData<SssGameData>();
            gdata.ChoiseModel = model;
            PlayerPrefs.SetInt(ChoiseModelKey, model);
        }


        protected void OnClickListener(GameObject gob)
        {
            MenuBtns btnid = (MenuBtns)UIEventListener.Get(gob).parameter;
            switch (btnid)
            {
                case MenuBtns.OpenListBtn:
                    OnClickOpenListBtn();
                    break;
                case MenuBtns.ChangeRoomBtn:

                    OnClickChangeRoomBtn();

                    break;
                case MenuBtns.SettingBtn:
                    OnClickSettingBtn();
                    break;
                case MenuBtns.QuitBtn:
                    QuitGame();
                    break;
                case MenuBtns.CloseListBtn:
                    CloseListView();
                    break;
                case MenuBtns.CloseSettingViewBtn:
                    OnClickCloseSettingViewBtn();
                    break;
                case MenuBtns.HistoryBtn:
                    OnClickHistoryBtn();
                    break;
                case MenuBtns.DismissRoom:
                    DismissRoom();
                    break;
                case MenuBtns.CloseHistoryBtn:
                    YxDebug.Log("关闭查看历史战绩界面");
                    break;
                case MenuBtns.RuleBtn:
                    OnClickRuleBtn();
                    break;
            }
        }



        [SuppressMessage("ReSharper", "InconsistentNaming")]
        enum MenuBtns
        {
            /// <summary>
            /// 打开菜单
            /// </summary>
            OpenListBtn,
            /// <summary>
            /// 换房间
            /// </summary>
            ChangeRoomBtn,
            /// <summary>
            /// 打开设置窗口
            /// </summary>
            SettingBtn,
            /// <summary>
            /// 退出游戏,返回大厅
            /// </summary>
            QuitBtn,
            /// <summary>
            /// 关闭下来菜单
            /// </summary>
            CloseListBtn,
            /// <summary>
            /// 关闭设置窗口
            /// </summary>
            CloseSettingViewBtn,
            /// <summary>
            /// 查看历史战绩
            /// </summary>
            HistoryBtn,
            /// <summary>
            /// 解散房间
            /// </summary>
            DismissRoom,
            /// <summary>
            /// 关闭查看历史按钮
            /// </summary>
            CloseHistoryBtn,
            /// <summary>
            /// 帮助按钮
            /// </summary>
            RuleBtn
        }
    }
}