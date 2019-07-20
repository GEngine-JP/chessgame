using UnityEngine;
using DG.Tweening;

namespace Assets.Scripts.Game.brnn3d
{
    public class BankerListBg : MonoBehaviour
    {
        public static BankerListBg Instance;
        public Transform BgTf;
        public Transform ListUITf;
        public bool IsZhanKai = true;

        protected void Awake()
        {
            Instance = this;

        }

        //显示庄家列表的UI
        public void ShowBankListUI()
        {
            if (IsZhanKai)
                return;
            Tweener tw = BgTf.DOLocalMoveY(-152, 0.4f);
            tw.OnComplete(delegate
                {
                    if (!ListUITf.gameObject.activeSelf)
                        ListUITf.gameObject.SetActive(true);
                    IsZhanKai = true;
                });
        }

        //隐藏庄家列表的UI
        public void HideBankListUI()
        {
            if (!IsZhanKai)
                return;
            if (ListUITf.gameObject.activeSelf)
                ListUITf.gameObject.SetActive(false);
            Tweener tw = BgTf.DOLocalMoveY(-70, 0.4f);
            tw.OnComplete(delegate
            {
                IsZhanKai = false;
            });
        }
    }
}

