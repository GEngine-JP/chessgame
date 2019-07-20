using UnityEngine;
using YxFramwork.Common;
using YxFramwork.View;

namespace Assets.Scripts.Game.brnn3d
{
    public class UpUIController : MonoBehaviour
    {
        public static UpUIController Instance;
        protected void Awake()
        {
            Instance = this;
        }
        //返回大厅
        public void ReturnToHall()
        {
            App.QuitGameWithMsgBox();
        }
        //上庄点击
        public void ApplyZhuangClicked()
        {
            ApplyXiaZhuangMgr.Instance.ApplyZhuangSendMsg();
        }

        //下庄点击
        public void XiaZhuangClicked()
        {
            ApplyXiaZhuangMgr.Instance.XiaZhuangSendMsg();
        }

        public void BankListUIOn_OffClick()
        {
            if (BankerListBg.Instance.IsZhanKai)
            {
                BankerListBg.Instance.HideBankListUI();
            }
            else
            {
                BankerListBg.Instance.ShowBankListUI();
            }
        }

    }
}

