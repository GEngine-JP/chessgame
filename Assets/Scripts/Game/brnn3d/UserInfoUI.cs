using UnityEngine;
using UnityEngine.UI;
using YxFramwork.Common;
using YxFramwork.Manager;

namespace Assets.Scripts.Game.brnn3d
{
    public class UserInfoUI : MonoBehaviour
    {
        public static UserInfoUI Instance;
        public Text UserNameText;
        public Text AchievementText;
        public Text MoneyText;

        protected void Awake()
        {
            Instance = this;
//            MusicManager.Instance.PlayBacksound("1");
//            MusicManager.Instance.EffectVolume = 1;
//            MusicManager.Instance.MusicVolume = 1;
        }

        private long ResultGold;
        /// <summary>
        /// 设置玩家信息
        /// </summary>
        public void SetUserInfoUI()
        {
            ResultGold += App.GetGameData<GlobalData>().ResultUserTotal;
            UserNameText.text = App.GetGameData<GlobalData>().CurrentUser.Name;
            AchievementText.text = ResultGold + "";
            MoneyText.text = App.GetGameData<GlobalData>().CurrentUser.Gold + "";
            App.GetGameData<GlobalData>().ResultUserTotal = 0;
        }

    }
}

