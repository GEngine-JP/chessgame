using Sfs2X.Entities.Data;
using UnityEngine;
using UnityEngine.UI;
using YxFramwork.Common;
using YxFramwork.Common.Utils;

namespace Assets.Scripts.Game.brnn3d
{
    public class BankerInfoUI : MonoBehaviour
    {
        public static BankerInfoUI Instance;
        public Text BankerNameText;
        public Text AchieveText;
        public Text MoneyText;
        public Text JuText;

        private int _record = -1;
        protected void Awake()
        {
            Instance = this;
        }

        //设置庄家信息UI
        public void SetBankerInfoUIData()
        {
            if (_record == -1)
            {
                _record = App.GetGameData<GlobalData>().B;
            }

            if (_record != App.GetGameData<GlobalData>().B)
            {
                App.GetGameData<GlobalData>().Frame++;
                App.GetGameData<GlobalData>().Bundle = 0;
                AchieveText.text = "0";
            }

            _record = App.GetGameData<GlobalData>().B;

            if (App.GetGameData<GlobalData>().BankList == null || App.GetGameData<GlobalData>().BankList.Size() == 0)
            {
                //App.GetGameData<GlobalData>().CurrentBanker.Seat = App.GetGameData<GlobalData>().B;
                BankerNameText.text = "";
                MoneyText.text = "";
                AchieveText.text = "";
                JuText.text = "";
                App.GetGameData<GlobalData>().CurrentBanker.Seat = App.GetGameData<GlobalData>().B;
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
                    BankerListUI.Instance.SetBankerListUI(user.Name, user.Gold + "");
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

                    App.GetGameData<GlobalData>().CurrentBanker = user;
                    BankerNameText.text = user.Name;
                    _resultGold += App.GetGameData<GlobalData>().ResultBnakerTotal;
                    AchieveText.text = _resultGold + "";
                    MoneyText.text = user.Gold + "";
                    JuText.text = "1";
                    App.GetGameData<GlobalData>().ResultBnakerTotal = 0;
                }
                else
                {
                    BankerListUI.Instance.SetBankerListUI(user.Name, user.Gold + "");
                }
            }

        }
        private long _resultGold;
    }
}

