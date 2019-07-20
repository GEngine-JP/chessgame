using UnityEngine;
using YxFramwork.Common;
using YxFramwork.Manager;

namespace Assets.Scripts.Game.brnn3d
{
    public class ApplyXiaZhuangMgr : MonoBehaviour
    {
        public static ApplyXiaZhuangMgr Instance;
        protected void Awake()
        {
            Instance = this;
        }
        //玩家上庄
        public void ApplyZhuangSendMsg()
        {
            if (CheckIsZhuang()) return;
            if (App.GetGameData<GlobalData>().CurrentUser.Gold < App.GetGameData<GlobalData>().Bkmingold)
                NoteUI.Instance.Note(string.Format(App.GetGameData<GlobalData>().ShangZhuangMoneyLos, App.GetGameData<GlobalData>().Bkmingold));
            else
            {
                App.GetRServer<GameServer>().ApplyBanker();
            }
        }

        //玩家下庄
        public void XiaZhuangSendMsg()
        {
            if (CheckIsZhuang())
            {
                App.GetRServer<GameServer>().ApplyQuit();
                if (App.GetGameData<GlobalData>().CurrentUser.Seat == App.GetGameData<GlobalData>().CurrentBanker.Seat)
                    NoteUI.Instance.Note(App.GetGameData<GlobalData>().NextXiaZuang);
            }
        }

        //设置申请上下装按钮的状态
        public void SetApplayXiaZhuangUIData()
        {
            SetApplyXiaZhuangUIDataEx(!CheckIsZhuang());
        }

        //判断是否是庄
        bool CheckIsZhuang()
        {
            if (App.GetGameData<GlobalData>().CurrentUser.Seat == App.GetGameData<GlobalData>().CurrentBanker.Seat)
            {
                return true;
            }
            var gdata = App.GetGameData<GlobalData>();
            if (gdata == null)
            {
                return false;
            }
            var bankList = gdata.BankList;
            if (bankList == null)
            {
                return false;
            }
            var count = bankList.Count;
            var seat = gdata.CurrentUser.Seat;
            for (int i = 0; i < count; i++)
            {
                if (bankList.GetSFSObject(i).GetInt("seat") == seat)
                {
                    return true;
                }
            }
            return false;
        }

        //设置上下庄的UI
        void SetApplyXiaZhuangUIDataEx(bool isApply)
        {
            //MusicManager.Instance.Play("UpAndDownRanker");
            if (isApply)
                ApplyXiaZhuangUI.Instance.ShowApplyZhuang();
            else
                ApplyXiaZhuangUI.Instance.ShowXiaZhuang();
        }
    }
}

