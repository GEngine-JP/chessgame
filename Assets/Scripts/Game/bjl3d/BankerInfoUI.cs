using Assets.Scripts.Game.bjl3d.Scripts;
using Sfs2X.Entities.Data;
using UnityEngine;
using UnityEngine.UI;
using YxFramwork.Common;
using YxFramwork.Common.Utils;
using com.yxixia.utile.YxDebug;

namespace Assets.Scripts.Game.bjl3d
{
    public class BankerInfoUI : MonoBehaviour// G 11.15
    {
        public static BankerInfoUI Instance;
        private Image _rankerHeadPortraitImg;
        private Text _rankerNameText;
        private Text _rankerGameCurrencyText;
        private Text _rankerAchievementText;
        private Text _gameNumber;


        public static BankerInfoUI Intance;
        /// <summary>
        /// 获取UI操作控件
        /// </summary>
        protected void Awake()
        {
            Instance = this;
            Transform tf = transform.FindChild("RankerHeadPortrait_Img");
            if (tf == null)
                YxDebug.LogError("No Such Object");//没有该物体    
            if (tf != null) _rankerHeadPortraitImg = tf.GetComponent<Image>();
            if (_rankerHeadPortraitImg == null)
                YxDebug.LogError("No Such  Component");//没有该组件

            tf = transform.FindChild("RankerName_Text");
            if (tf == null)
                YxDebug.LogError("No Such Object");//没有该物体    
            if (tf != null) _rankerNameText = tf.GetComponent<Text>();
            if (_rankerNameText == null)
                YxDebug.LogError("No Such  Component");//没有该组件

            tf = transform.FindChild("RankerGameCurrency_Text");
            if (tf == null)
                YxDebug.LogError("No Such Object");//没有该物体    
            if (tf != null) _rankerGameCurrencyText = tf.GetComponent<Text>();
            if (_rankerGameCurrencyText == null)
                YxDebug.LogError("No Such  Component");//没有该组件


            tf = transform.FindChild("RankerAchievement_Text");
            if (tf == null)
                YxDebug.LogError("No Such Object");//没有该物体    
            if (tf != null) _rankerAchievementText = tf.GetComponent<Text>();
            if (_rankerAchievementText == null)
                YxDebug.LogError("No Such  Component");//没有该组件


            tf = transform.FindChild("RankerGameNumber_Text");
            if (tf == null)
                YxDebug.LogError("No Such Object");//没有该物体    
            if (tf != null) _gameNumber = tf.GetComponent<Text>();
            if (_gameNumber == null)
                YxDebug.LogError("No Such  Component");//没有该组件

            Intance = this;
        }

        private long _resultGold;


        private bool _isChangezhuang = true;
        private int _record = -1;
        /// <summary>
        /// 游戏运行了多少局
        /// </summary>
        public void GameInnings()
        {
            UserInfoUI.Instance.GameConfig.GameNum++;
            if (_isChangezhuang)
            {
                UserInfoUI.Instance.GameConfig.GameNum = 1;
                _gameNumber.text = UserInfoUI.Instance.GameConfig.GameNum + "";
            }
            else
            {
                _gameNumber.text = UserInfoUI.Instance.GameConfig.GameNum + "";
            }

        }
        /// <summary>
        ///  显示等待上庄的玩家信息
        /// </summary>
        public void ShowUserInfoUI()
        {
            if (_record == -1)
            {
                _record = App.GetGameData<GlobalData>().B;
            }

            if (_record != App.GetGameData<GlobalData>().B)
            {
                _isChangezhuang = true;
            }
            else
            {
                _isChangezhuang = false;
            }
            if (App.GetGameData<GlobalData>().BankList == null || App.GetGameData<GlobalData>().BankList.Size() == 0)
            {
                App.GetGameData<GlobalData>().CurrentBanker.Seat = App.GetGameData<GlobalData>().B;
                _rankerNameText.text = "系统庄";
//                _resultGold += App.GetGameData<GlobalData>().ResultBnakerTotal;
                _rankerAchievementText.text = /*_resultGold +*/ "";
                _rankerGameCurrencyText.text = "";
//                App.GetGameData<GlobalData>().ResultBnakerTotal = 0;
                return;
            }
            foreach (ISFSObject banber in App.GetGameData<GlobalData>().BankList)
            {
                UserInfo user = new UserInfo();

                user.Seat = banber.GetInt("seat");
                user.Gold = banber.GetLong("ttgold");
                user.Name = banber.GetUtfString("username");
                if (user.Seat == App.GetGameData<GlobalData>().B)
                {
                    WaitForRankerListUI.Instance.ShowRankerListUI(user.Name, user.Gold + "");
                }
            }

            foreach (ISFSObject banber in App.GetGameData<GlobalData>().BankList)
            {
                UserInfo user = new UserInfo();

                user.Seat = banber.GetInt("seat");
                user.Gold = banber.GetLong("ttgold");
                user.Name = banber.GetUtfString("username");
                App.GetGameData<GlobalData>().CurrentBanker.Seat = App.GetGameData<GlobalData>().B;
                if (user.Seat == App.GetGameData<GlobalData>().B)
                {
                    App.GameData.GStatus = GameStatus.PlayAndConfine;
                    App.GetGameData<GlobalData>().CurrentBanker = user;
                    _rankerNameText.text = user.Name;
//                    _resultGold += App.GetGameData<GlobalData>().ResultBnakerTotal;
//                    _rankerAchievementText.text = _resultGold + "";
                    _rankerAchievementText.text = App.GetGameData<GlobalData>().ResultBnakerTotal+"";
                    _rankerGameCurrencyText.text = user.Gold + "";
//                    App.GetGameData<GlobalData>().ResultBnakerTotal = 0;
                }
                else
                {
                    WaitForRankerListUI.Instance.ShowRankerListUI(user.Name, user.Gold + "");
                }
            }


        }
    }
}