using UnityEngine;
using UnityEngine.UI;
using YxFramwork.Common;
using YxFramwork.Framework.Core;
using YxFramwork.Manager;

namespace Assets.Scripts.Game.bjl3d
{
    public class UserInfoUI : MonoBehaviour// g 11.15
    {
        public static UserInfoUI Instance;

        public Image UserHeadPortraitImg;
        public Text UserNameText;
        public Text UserGameCurrencyText;
        public Text UserAchievementText;
        public Text UserHitRateText;
        public GameConfig GameConfig;

        protected void Awake()
        {
            Instance = this;
            GameConfig = new GameConfig();
        }
        protected void Start(){
            var musicMgr = Facade.Instance<MusicManager>();
//            musicMgr.PlayBacksound("BG3");
            musicMgr.MusicVolume = 1;
            musicMgr.EffectVolume = 1;
        }

        /// <summary>
        /// UI显示玩家信息
        /// </summary>
        public void ShowSelfInfoUI()
        {
            UserNameText.text = App.GetGameData<GlobalData>().CurrentUser.Name;
            UserGameCurrencyText.text = App.GetGameData<GlobalData>().CurrentUser.Gold + "";
//            if (App.GetGameData<GlobalData>().CurrentUser.Seat == App.GetGameData<GlobalData>().B)
//            {
//                UserMoney += (int)App.GetGameData<GlobalData>().ResultBnakerTotal;
//                UserAchievementText.text = UserMoney + "";
//            }
//            else
//            {
                UserAchievementText.text = App.GetGameData<GlobalData>().TodayWin + "";
//            }
            UserHitRateText.text = "";
//            App.GetGameData<GlobalData>().ResultBnakerTotal=0;
        }

        private int UserMoney;
        void OnDestory()
        {
            Instance = null;
        } 
    }

}