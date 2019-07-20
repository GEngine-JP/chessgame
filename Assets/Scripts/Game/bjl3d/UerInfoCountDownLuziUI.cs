using UnityEngine;
using System.Collections;
using YxFramwork.Manager;
using com.yxixia.utile.YxDebug;

namespace Assets.Scripts.Game.bjl3d
{//打开和关闭Down菜单
    public class UerInfoCountDownLuziUI : MonoBehaviour//G 11.15
    {

        public static UerInfoCountDownLuziUI Intance;

        private Transform _buttonImgtf;//隐藏和显示开关
        private bool _isShowing = true;
        private Animator _ani;
        private Transform _coinTypeInfoTf;//下注

        protected void Awake()
        {
            Intance = this;
            _ani = transform.GetComponent<Animator>();
            if (_ani == null)
                YxDebug.LogError("No such Component");

            _buttonImgtf = transform.Find("HideUserInfoCountDownLuziUIBtn/buttonImgPoint");
            if (_buttonImgtf == null)
                YxDebug.LogError("No Such transform");

            _coinTypeInfoTf = transform.Find("CoinTypeInfo");
            if (_coinTypeInfoTf == null)
                YxDebug.LogError("No Such transform");
        }
        /// <summary>
        /// 打开下菜单
        /// </summary>
        public void ShowUIFun()
        {
            _coinTypeInfoTf.gameObject.SetActive(true);
            if (_isShowing)
                return;
            _ani.Play("shangshen");
            _buttonImgtf.localEulerAngles = new Vector3(0, 0, 0);
            _isShowing = true;
        }
        /// <summary>
        /// 隐藏界面摄像机底部的菜单框
        /// </summary>
        /// <param name="isShowCoinType"></param>
        public void HideUIFun(bool isShowCoinType = true)
        {
            _coinTypeInfoTf.gameObject.SetActive(isShowCoinType);
            if (!_isShowing)
                return;
            _ani.Play("xialuo");
            _buttonImgtf.localEulerAngles = new Vector3(180, 0, 0);
            MusicManager.Instance.Play("ShakeUI");
            //            AudioClip clip = ResourcesLoader.instance.LoadAudio("ShakeUI");..........
            //            AudioManager.Instance.Play(clip, false, .8f);.............
            StartCoroutine("DownUIAudio");
            DownUI.Intance.ShowYanEff();   //闪动图片     
            _isShowing = false;
        }

        IEnumerator DownUIAudio()
        {
            yield return new WaitForSeconds(0.5f);
            MusicManager.Instance.Play("UIDown");
            //            AudioClip clip = ResourcesLoader.instance.LoadAudio("UIDown");..............
            //            AudioManager.Instance.Play(clip, false, .8f);.............
        }

        public void ClickeUIFun()
        {
            if (_isShowing)
                HideUIFun();
            else
                ShowUIFun();
        }

    }
}
