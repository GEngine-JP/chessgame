using System.Collections;
using UnityEngine;
using YxFramwork.Common;
using YxFramwork.Manager;

namespace Assets.Scripts.Game.brnn3d
{
    public class BetModeMgr : MonoBehaviour
    {
        public static BetModeMgr Instance;
        private int _coinType;
        protected void Awake()
        {
            Instance = this;
        }

        //设置下注筹码的数据
        public void SetBetModeChouMaData()
        {
            MusicManager.Instance.Stop();
            MusicManager.Instance.Play("xiazhubeijing");

            _coinType = GetCoinTypeByChouMaMoney(App.GetGameData<GlobalData>().BetMoney);

            SetBetModeChouMaDataEx();
            SetBetMoneyUIShowDataEx();//下注钱数界面的变化显示
        }
        int GetCoinTypeByChouMaMoney(int money)
        {
            int cointype = -1;
            switch (money)
            {
                case 1:
                    cointype = 0;
                    break;
                case 5:
                    cointype = 1;
                    break;
                case 10:
                    cointype = 2;
                    break;
                case 50:
                    cointype = 3;
                    break;
                case 100:
                    cointype = 4;
                    break;
                case 500:
                    cointype = 5;
                    break;
                case 1000:
                    cointype = 6;
                    break;
                case 5000:
                    cointype = 7;
                    break;
                case 10000:
                    cointype = 8;
                    break;
                case 50000:
                    cointype = 9;
                    break;
                case 100000:
                    cointype = 10;
                    break;
                case 500000:
                    cointype = 11;
                    break;
                case 1000000:
                    cointype = 12;
                    break;
                case 5000000:
                    cointype = 13;
                    break;
                case 10000000:
                    cointype = 14;
                    break;
            }
            return cointype;
        }

        //设置下注筹码的效果
        public void SetBetModeChouMaDataEx()
        {
            BetMode.Instance.InstanceCoinDemo(_coinType, App.GetGameData<GlobalData>().BetPos, App.GetGameData<GlobalData>().UserSeat);
        }
        public bool IsSelf;

        public void DisPlay()
        {
            StartCoroutine("Wait");
        }

        private IEnumerator Wait()
        {
            yield return new WaitForSeconds(2f);
        }

        //下注钱数界面的变化显示
        public void SetBetMoneyUIShowDataEx()
        {
            if (App.GetGameData<GlobalData>().CurrentUser.Seat == App.GetGameData<GlobalData>().UserSeat)
            {
                IsSelf = true;
            }
            BetMoneyUI.Instance.SetBetMoneyUI(IsSelf);
            IsSelf = false;
        }

    }
}

