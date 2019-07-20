using Assets.Scripts.Game.lswc.Control.System;
using Assets.Scripts.Game.lswc.Core;
using Assets.Scripts.Game.lswc.Data;
using UnityEngine.UI;
using YxFramwork.Common;

namespace Assets.Scripts.Game.lswc.Manager
{
    /// <summary>
    /// 交互控制类，主要控制按钮的触发
    /// </summary>
    public class LSOperationManager : InstanceControl
    {
        private static LSOperationManager _instance;

        public static LSOperationManager Instance
        {
            get { return _instance; }
        }

        public Button BackBtn;

        public Button ExpandBtn;

        public Button SharinkBtn;

        public Button SettingBtn;

        public Button ChangeAnteBtn;

        public Button GoOnBetBtn;

        public Button ClearBtn;

        public Button AllInBtn;

        private void Awake()
        {
            _instance = this;
        }

        public void InitListener()
        {
            BackBtn.onClick.AddListener(OnClickBackbtn);
            ExpandBtn.onClick.AddListener(OnClickExpandBtn);
            SharinkBtn.onClick.AddListener(OnclickSharinkBtn);
            SettingBtn.onClick.AddListener(OnClickSettingBtn);
            ChangeAnteBtn.onClick.AddListener(OnClickChangeAnteBtn);
            GoOnBetBtn.onClick.AddListener(OnClickGoOnBtn);
            ClearBtn.onClick.AddListener(OnClickClearBtn);
            AllInBtn.onClick.AddListener(OnClickAllInBtn);
        }

        private void OnClickBackbtn()
        {
           LSSystemControl.Instance.QuitGame();
        }

        private void OnClickExpandBtn()
        {
            LSUIManager.Instance.ShowBetWindow();
        }

        private void OnclickSharinkBtn()
        {
            LSUIManager.Instance.HideBetWindow();
        }

        private void OnClickSettingBtn()
        {
            LSUIManager.Instance.ShowSettingWindow();
        }

        private void OnClickChangeAnteBtn()
        {
            App.GetGameData<GlobalData>().ChangeAnte();
            LSUIManager.Instance.ChangeAnte();
            LSSystemControl.Instance.PlaySuccess(true);
        }

        private void OnClickGoOnBtn()
        {
            bool success = false;
            if (App.GetGameData<GlobalData>().GlobalGameStatu == GameState.BetState)
            {
                success = App.GetGameData<GlobalData>().BetAgain();
                if (success)
                {
                    LSUIManager.Instance.SetBetWindow();
                }
            }
            LSSystemControl.Instance.PlaySuccess(success);
        }

        private void OnClickClearBtn()
        {
            bool success = false;
            if (App.GetGameData<GlobalData>().GlobalGameStatu == GameState.BetState)
            {
                App.GetGameData<GlobalData>().ClearBets();
                LSUIManager.Instance.SetBetWindow();
                success = true;
            }
            LSSystemControl.Instance.PlaySuccess(success);

        }

        private void OnClickAllInBtn()
        {
            bool success = false;
            if (App.GetGameData<GlobalData>().GlobalGameStatu == GameState.BetState)
            {
                if (App.GetGameData<GlobalData>().BetAll())
                {
                    success = true;
                }       
                LSUIManager.Instance.SetBetWindow();
            }
            LSSystemControl.Instance.PlaySuccess(success);
        }

        public override void OnExit()
        {
            _instance = null;
            BackBtn.onClick.RemoveAllListeners();
            ExpandBtn.onClick.RemoveAllListeners();
            SharinkBtn.onClick.RemoveAllListeners();
            SettingBtn.onClick.RemoveAllListeners();
            ChangeAnteBtn.onClick.RemoveAllListeners();
            GoOnBetBtn.onClick.RemoveAllListeners();
            ClearBtn.onClick.RemoveAllListeners();
            AllInBtn.onClick.RemoveAllListeners();
        }
    }
}
