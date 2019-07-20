using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using Assets.Scripts.Game.ddz2.DDz2Common;
using Assets.Scripts.Game.ddz2.DdzEventArgs;
using Assets.Scripts.Game.ddz2.InheritCommon;
using UnityEngine;
using YxFramwork.Common;
using YxFramwork.ConstDefine;

namespace Assets.Scripts.Game.ddz2.DDzGameListener.ClockPointPanel
{
    public class ClockPintListener : ServEvtListener
    {
        [SerializeField]
        protected GameObject ClockBgGob;
        
        [SerializeField] 
        protected UISprite PointSelf;
        [SerializeField]
        protected UISprite PointRight;
        [SerializeField]
        protected UISprite PointLeft;

        [SerializeField]
        protected UILabel CuntDownLabel;

        /// <summary>
        /// 地主皇冠动画
        /// </summary>
        [SerializeField] protected GameObject DizhuHuangGuanGob;

        /// <summary>
        /// 静态地主图片
        /// </summary>
        [SerializeField] protected GameObject DiZhuTextureSprite;

        /// <summary>
        /// 皇冠粒子特效播放时长
        /// </summary>
        private float _huangGuanPlayTime = 3f;

        private Vector3 _selfPlayerPos = new Vector3(-590,5,0);
        private Vector3 _leftPlayerPos = new Vector3(-600, 187, 0);
        private Vector3 _rightPlayerPos = new Vector3(598, 187, 0);
        protected override void OnAwake()
        {

            Ddz2RemoteServer.AddOnGetRejoinDataEvt(OnRejoinGame);
            Ddz2RemoteServer.AddOnServResponseEvtDic(GlobalConstKey.TypeFirstOut, OnTypeFirstOut);
            Ddz2RemoteServer.AddOnServResponseEvtDic(GlobalConstKey.TypeOutCard, OnTypeOutCard);
            Ddz2RemoteServer.AddOnServResponseEvtDic(GlobalConstKey.TypePass, OnTypePass);
            Ddz2RemoteServer.AddOnServResponseEvtDic(GlobalConstKey.TypeGameOver, OnTypeGameOver);
        }

        void Start()
        {
            //游戏退出到大厅时，清理可能引起程序崩溃的资源
            App.GetGameData<GlobalData>().OnClearParticalGob = ClearParticalGob;
            _huangGuanPlayTime = DDzUtil.ParticleSystemLength(DizhuHuangGuanGob.transform);
            HideAllGobs();
        }

        private void HideAllGobs()
        {
            ClockBgGob.SetActive(false);
            PointSelf.gameObject.SetActive(false);
            PointRight.gameObject.SetActive(false);
            PointLeft.gameObject.SetActive(false);
            CuntDownLabel.gameObject.SetActive(false);

            DizhuHuangGuanGob.SetActive(false);
            DiZhuTextureSprite.SetActive(false);
        }

        private void ClearParticalGob()
        {
            DestroyImmediate(DizhuHuangGuanGob);
        }

        public override void RefreshUiInfo()
        {
            //throw new System.NotImplementedException();
        }


        private void OnRejoinGame(object sender, DdzbaseEventArgs args)
        {
            var data = args.IsfObjData;

            //没人行动，隐藏所有
            if (!data.ContainsKey(NewRequestKey.KeyCurrp))
            {
                HideAllGobs();
                return;
            }

            ShowPointAndCuntDown(data.GetInt(NewRequestKey.KeyCurrp));

            DizhuHuangGuanGob.SetActive(false);

            DiZhuTextureSprite.SetActive(false);
        }




        /// <summary>
        /// 当确定地主后，看自己是不是地主，来判断是否显示按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void OnTypeFirstOut(object sender, DdzbaseEventArgs args)
        {
            StartCoroutine(PlayerGiveDizhuAnim(args.IsfObjData.GetInt(RequestKey.KeySeat)));
        }

        private IEnumerator PlayerGiveDizhuAnim(int dizhuSeat)
        {
            if (DizhuHuangGuanGob==null) yield break;
            DiZhuTextureSprite.SetActive(false);
            DizhuHuangGuanGob.SetActive(false);
            DizhuHuangGuanGob.SetActive(true);
            var particalHuanguan = DizhuHuangGuanGob.GetComponent<ParticleSystem>();
            particalHuanguan.Stop();
            particalHuanguan.Clear();
            particalHuanguan.Play();

            yield return new WaitForSeconds(_huangGuanPlayTime);
            DizhuHuangGuanGob.SetActive(false);

            DiZhuTextureSprite .SetActive(true);
            var dizhuTexturetween = DiZhuTextureSprite.GetComponent<TweenPosition>();
            dizhuTexturetween.ResetToBeginning();
            dizhuTexturetween.from = new Vector3(0,0,0);

            if (dizhuSeat == App.GetGameData<GlobalData>().GetSelfSeat)
                dizhuTexturetween.to = _selfPlayerPos;
            else if (dizhuSeat == App.GetGameData<GlobalData>().GetLeftPlayerSeat)
                dizhuTexturetween.to = _leftPlayerPos;
            else if (dizhuSeat == App.GetGameData<GlobalData>().GetRightPlayerSeat)
                dizhuTexturetween.to = _rightPlayerPos;
            dizhuTexturetween.PlayForward();
            dizhuTexturetween.onFinished.Clear();
            dizhuTexturetween.AddOnFinished(() => DiZhuTextureSprite.SetActive(false));

            ShowPointAndCuntDown(dizhuSeat);
        }

        /// <summary>
        /// 当有人出牌时
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void OnTypeOutCard(object sender, DdzbaseEventArgs args)
        {
            AfterSomeBodyAction(args.IsfObjData.GetInt(RequestKey.KeySeat));
        }


        /// <summary>
        /// 有人pass的时候
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void OnTypePass(object sender, DdzbaseEventArgs args)
        {
            AfterSomeBodyAction(args.IsfObjData.GetInt(RequestKey.KeySeat));
        }


        /// <summary>
        /// 当游戏结算时
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        protected virtual void OnTypeGameOver(object sender, DdzbaseEventArgs args)
        {
            HideAllGobs();
        }

        /// <summary>
        /// 当某人行动时要进行的动作
        /// </summary>
        private void AfterSomeBodyAction(int actionPlayerSeat)
        {
            var selfSeat = App.GetGameData<GlobalData>().GetSelfSeat;
            var leftSeat = App.GetGameData<GlobalData>().GetLeftPlayerSeat;
            var rightSeat = App.GetGameData<GlobalData>().GetRightPlayerSeat;

            if (actionPlayerSeat == selfSeat) ShowPointAndCuntDown(rightSeat);
            else if (actionPlayerSeat == rightSeat) ShowPointAndCuntDown(leftSeat);
            else if (actionPlayerSeat == leftSeat) ShowPointAndCuntDown(selfSeat);

        }

        /// <summary>
        /// 箭头指向方位和倒计时开始
        /// </summary>
        /// <param name="playerSeat"></param>
        private void ShowPointAndCuntDown(int playerSeat)
        {
            HideAllPoints();
            if (playerSeat == App.GetGameData<GlobalData>().GetSelfSeat) PointSelf.gameObject.SetActive(true);
            else if (playerSeat == App.GetGameData<GlobalData>().GetRightPlayerSeat) PointRight.gameObject.SetActive(true);
            else if (playerSeat == App.GetGameData<GlobalData>().GetLeftPlayerSeat) PointLeft.gameObject.SetActive(true);

            ClockBgGob.SetActive(true);

            StopAllCoroutines();
            StartCoroutine(ReClock(10));
        }

        private IEnumerator ReClock(int cuntTime)
        {
            CuntDownLabel.gameObject.SetActive(true);
            while (cuntTime>0)
            {
                CuntDownLabel.text = cuntTime.ToString(CultureInfo.InvariantCulture);
                yield return new WaitForSeconds(1);
                cuntTime--;
            }
            CuntDownLabel.text = "0";
        }


        /// <summary>
        /// 隐藏所有
        /// </summary>
        private void HideAllPoints()
        {
            PointSelf.gameObject.SetActive(false);
            PointRight.gameObject.SetActive(false);
            PointLeft.gameObject.SetActive(false);
        }
    }
}
