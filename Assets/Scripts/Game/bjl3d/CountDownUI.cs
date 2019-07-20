using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using YxFramwork.Common;
using YxFramwork.Manager;
using com.yxixia.utile.YxDebug;

namespace Assets.Scripts.Game.bjl3d
{
    public class CountDownUI : MonoBehaviour// G  11  15
    {
        public static CountDownUI Instance;

        public Sprite[] Numbers;

        private int _timecount;
        // private Text TimeCountDownText;

        private Transform _szApplyRankerEff;
        private Transform _xzApplyRankerEff;

        private Transform _szApplyRankerBtn;
        private Transform _xzApplyRankerBtn;

        public Image SiImage;
        public Image GeImage;

        /// <summary>
        /// 获取UI操作控件
        /// </summary>
        protected void Awake()
        {
            Instance = this;
            _szApplyRankerEff = transform.FindChild("Particle System_sz");
            if (_szApplyRankerEff == null)
                YxDebug.LogError("No Such Object");//没有该物体 

            _xzApplyRankerEff = transform.FindChild("Particle System_xz");
            if (_xzApplyRankerEff == null)
                YxDebug.LogError("No Such Object");//没有该物体 

            _szApplyRankerBtn = transform.FindChild("shangzhuang");
            if (_szApplyRankerBtn == null)
                YxDebug.LogError("No Such Object");//没有该物体 

            _xzApplyRankerBtn = transform.FindChild("xiazhuang");
            if (_xzApplyRankerBtn == null)
                YxDebug.LogError("No Such Object");//没有该物体 


            Transform tf = transform.FindChild("NumBg/SiImg");
            if (tf == null)
                YxDebug.LogError("No Such Object");//没有该物体    
            if (tf != null) SiImage = tf.GetComponent<Image>();
            if (SiImage == null)
                YxDebug.LogError("No Such  Component");//没有该组件

            tf = transform.FindChild("NumBg/GeImg");
            if (tf == null)
                YxDebug.LogError("No Such Object");//没有该物体    
            if (tf != null) GeImage = tf.GetComponent<Image>();
            if (GeImage == null)
                YxDebug.LogError("No Such  Component");//没有该组件
        }

        /// <summary>
        /// 下注
        /// </summary>
        public void NoticeXiaZhuFun()
        {
            YxDebug.Log("下注");

            _timecount = UserInfoUI.Instance.GameConfig.XiaZhuTime;//下注时间 15
            UserInfoUI.Instance.GameConfig.IsXiaZhuTime = true;//是否是下注时间
            CameraMgr.Instance.CameraMoveByPath(0);
            UerInfoCountDownLuziUI.Intance.HideUIFun();
            BetMoneyUI.Intance.BetMoneyArea();
        }
        /// <summary>
        /// 发牌
        /// </summary>
        public void SendCardFun()
        {
            YxDebug.Log("send card...");

            _timecount = UserInfoUI.Instance.GameConfig.KaiPaiTime;
            UserInfoUI.Instance.GameConfig.IsXiaZhuTime = false;
            CameraMgr.Instance.CameraMoveByPath(1);
            UerInfoCountDownLuziUI.Intance.HideUIFun(false);
            BetMoneyUI.Intance.BetMoneyQingKongInfo();
            StartCoroutine("SendCardMoveCameraToDo", 3f);
        }

        IEnumerator SendCardMoveCameraToDo(float s)
        {
            yield return new WaitForSeconds(s);
            PlanScene.Instance.QingKongChouma();
        }
        /// <summary>
        /// 中奖区域（赢得）
        /// </summary>
        public void ShowWinAreasFun()
        {
            _timecount = UserInfoUI.Instance.GameConfig.ShowWinTime;
            UserInfoUI.Instance.GameConfig.IsXiaZhuTime = false;
            CameraMgr.Instance.CameraMoveByPath(4);

            ShowArea();
            MusicManager.Instance.Play("win");
            //            AudioClip clip = ResourcesLoader.instance.LoadAudio("music/win");
            //            AudioManager.Instance.Play(clip, false, .8f);
            UerInfoCountDownLuziUI.Intance.HideUIFun();
        }
        /// <summary>
        /// 打开下拉菜单
        /// </summary>
        public void GameResultFun()
        {
            Debug.Log("jie suans...");
            _timecount = UserInfoUI.Instance.GameConfig.XiaZhuTime;
            UserInfoUI.Instance.GameConfig.IsXiaZhuTime = false;
            UerInfoCountDownLuziUI.Intance.ShowUIFun();//打开下拉菜单
            UserInfoUI.Instance.GameConfig.XFapaiSpeedflag = 0;
        }


        /// <summary>
        /// 中奖显示区域
        /// </summary>
        void ShowArea()
        {
            for (int i = 0; i < App.GetGameData<GlobalData>().BetJiesuan.Length; i++)
            {
                if (App.GetGameData<GlobalData>().BetJiesuan[i] != 0)
                {
                    if (GameScene.Instance.WinAreaEffs[i].gameObject.activeSelf)
                        GameScene.Instance.WinAreaEffs[i].gameObject.SetActive(false);
                    GameScene.Instance.WinAreaEffs[i].gameObject.SetActive(true);
                }
            }
        }

        private float _time;
        protected void Update()
        {
            _time += Time.deltaTime;
            if (_time >= 1.0f)
            {
                if (_timecount > 0)
                    _timecount = _timecount - 1;
                GetTimeCountNumberToImg(_timecount);
                if (UserInfoUI.Instance.GameConfig.IsXiaZhuTime && _timecount < 4)
                {
                    MusicManager.Instance.Play("timeout");
                    //AudioClip clip = ResourcesLoader.instance.LoadAudio("music/timeout");
                    //AudioManager.Instance.Play(clip, false, .8f);
                }
                _time = 0f;
            }
        }
        /// <summary>
        /// 倒计时
        /// </summary>
        /// <param name="count"></param>
        public void GetTimeCountNumberToImg(int count)
        {
            int shiN = count / 10;
            SiImage.sprite = Numbers[shiN];
            int geN = count % 10;
            GeImage.sprite = Numbers[geN];
        }

        private bool _isApply;
        //上下庄申请
        public void ApplyToRankerBtn()
        {
            if (App.GetGameData<GlobalData>().CurrentUser.Gold < App.GetGameData<GlobalData>().BankLimit)
            {
                GameUI.Instance.NoteText_Show("金币不足！！！");
                return;
            }
            if (App.GetGameData<GlobalData>().CurrentUser.Seat== App.GetGameData<GlobalData>().CurrentBanker.Seat)
            {
                GameUI.Instance.NoteText_Show("这把游戏结束后自动下庄！！！");
                ShowS_X_Image(_isApply);
                App.GetRServer<GameServer>().ApplyQuit();//向服务区器发送下庄请求
                return;
            }

            MusicManager.Instance.Play("UpAndDownRanker");
            //            AudioClip clip = ResourcesLoader.instance.LoadAudio("UpAndDownRanker");
            //            AudioManager.Instance.Play(clip, false, .8f);

            if (WaitForRankerListUI.Instance.IsApplyRankerOrXiaRanker)//true 可以上庄  flase 不可以上庄
            {

                if (_szApplyRankerEff.gameObject.activeSelf)
                    _szApplyRankerEff.gameObject.SetActive(false);
                _szApplyRankerEff.gameObject.SetActive(true);
                App.GetRServer<GameServer>().ApplyBanker();//向服务区器发送上庄请求
                _isApply = false;
            }
            else
            {
                if (_xzApplyRankerEff.gameObject.activeSelf)
                    _xzApplyRankerEff.gameObject.SetActive(false);
                _xzApplyRankerEff.gameObject.SetActive(true);
                App.GetRServer<GameServer>().ApplyQuit();//向服务区器发送下庄请求
                _isApply = true;
            }
            ShowS_X_Image(_isApply);
        }
        /// <summary>
        /// 上下庄图片显示
        /// </summary>
        /// <param name="isApply"></param>
        public void ShowS_X_Image(bool isApply)
        {
            if (isApply)
            {
                WaitForRankerListUI.Instance.IsApplyRankerOrXiaRanker = true;
                _xzApplyRankerBtn.gameObject.SetActive(false);
                if (_szApplyRankerBtn.gameObject.activeSelf)
                    _szApplyRankerBtn.gameObject.SetActive(false);
                _szApplyRankerBtn.gameObject.SetActive(true);
            }
            else
            {
                WaitForRankerListUI.Instance.IsApplyRankerOrXiaRanker = false;
                _szApplyRankerBtn.gameObject.SetActive(false);
                if (_xzApplyRankerBtn.gameObject.activeSelf)
                    _xzApplyRankerBtn.gameObject.SetActive(false);
                _xzApplyRankerBtn.gameObject.SetActive(true);
            }
        }


    }
}