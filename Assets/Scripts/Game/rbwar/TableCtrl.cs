using UnityEngine;
using YxFramwork.Common;
using YxFramwork.Enums;
using YxFramwork.Manager;
using YxFramwork.View;

namespace Assets.Scripts.Game.rbwar
{
    public class TableCtrl : MonoBehaviour
    {
        public void ReturnHall()
        {
            if (App.GameData.GStatus == YxEGameStatus.Normal)
            {
                App.QuitGame();
            }
            else
            {
                YxMessageBox.Show("正在游戏中,请稍后退出");
            }
           
        }

        public void OnShowSetting()
        {
            YxWindowManager.OpenWindow("SettingWindow");
        }

        public void OnShowRule()
        {
            YxWindowManager.OpenWindow("RuleWindow");
        }

        public void OnShowRank()
        {
            YxWindowManager.OpenWindow("RankWindow");
        }

    }
}
