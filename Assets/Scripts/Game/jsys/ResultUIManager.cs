using UnityEngine;
using UnityEngine.UI;
using YxFramwork.Common;
using YxFramwork.Manager;

namespace Assets.Scripts.Game.jsys
{
    public class ResultUIManager : MonoBehaviour
    {
        public static ResultUIManager Instance;

        public Text TotalInText;
        public Text WinText;
        public Image ResultPanel;

        public void Awake()
        {
            Instance = this;
            TotalInText.text = "0";
        }

        public bool isTrue = true;
        /// <summary>
        /// 游戏结束
        /// </summary>
        public void GameFinish()
        {
            App.GetGameData<GlobalData>().isOut = true;
            //隐藏开奖动画
            AnimationManager.Instance.HideAnimation();
            AnimationManager.Instance.HideBetPanel();
            //结算板
            if (App.GetGameData<GlobalData>().IsShark == false)
            {
                ShowJieSuan();
            }
            TotalInText.text = -App.GetGameData<GlobalData>().Ante + "";
            WinText.text = App.GetGameData<GlobalData>().Gold + "";
            BetPanelManager.Instance.ShowIgetMoney(App.GetGameData<GlobalData>().Gold);
            //更新路子显示
            HistoryManager.Instance.ShowNewHistory(App.GetGameData<GlobalData>().EndAnimal);
            if (App.GetGameData<GlobalData>().IsShark)
            {
                App.GetGameData<GlobalData>().IsShark = false;
                TurnGroupsManager.Instance.PlayGame();
                isTrue = false;
            }
            if (isTrue)
            {
                Invoke("ChuXian", 4f);
            }
            isTrue = true;
            App.GetGameData<GlobalData>().IsShark = false;
            App.GetGameData<GlobalData>().FishIdx = 1;
        }
        public void ChuXian()
        {
            BetPanelManager.Instance.GameBeginXizhu();
        }

        //显示开奖结算页面
        public void ShowJieSuan()
        {
            if (!ResultPanel.gameObject.activeSelf)
            {
                ResultPanel.gameObject.SetActive(true);
            }
        }
        //隐藏结算面板显示
        public void HideJieSuanUI()
        {
            if (ResultPanel.gameObject.activeSelf)
                ResultPanel.gameObject.SetActive(false);
        }
    }
}
