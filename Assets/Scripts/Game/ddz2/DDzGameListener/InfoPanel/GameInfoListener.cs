using System;
using System.Collections;
using System.Globalization;
using Assets.Scripts.Game.ddz2.DDz2Common;
using Assets.Scripts.Game.ddz2.DdzEventArgs;
using Assets.Scripts.Game.ddz2.InheritCommon;
using Assets.Scripts.Game.ddz2.PokerCdCtrl;
using Sfs2X.Entities.Data;
using UnityEngine;
using YxFramwork.Common;
using YxFramwork.ConstDefine;
using Assets.Scripts.Game.ddz2.PokerRule;
using com.yxixia.utile.YxDebug;

namespace Assets.Scripts.Game.ddz2.DDzGameListener.InfoPanel
{
    /// <summary>
    /// 处理游戏局数，低分，倍数等公共游戏信息
    /// </summary>
    public class GameInfoListener : ServEvtListener
    {
        protected override void OnAwake()
        {
            Ddz2RemoteServer.AddOnGameInfoEvt(OnGameInfo);
            Ddz2RemoteServer.AddOnGetRejoinDataEvt(OnRejoinGame);
            Ddz2RemoteServer.AddOnServResponseEvtDic(GlobalConstKey.TypeFirstOut, TypeFirstOut);
            Ddz2RemoteServer.AddOnUserReadyEvt(OnUserReady);
            Ddz2RemoteServer.AddOnchangeDizhu(OnChangDizhu);
            Ddz2RemoteServer.AddOnServResponseEvtDic(GlobalConstKey.TypeDoubleOver, OnDoubleOver);
            Ddz2RemoteServer.AddOnServResponseEvtDic(GlobalConstKey.TypeGrab, OnTypeGrab);
            Ddz2RemoteServer.AddOnServResponseEvtDic(GlobalConstKey.TypeOutCard, OnTypeOutCard);
            Ddz2RemoteServer.AddOnServResponseEvtDic(GlobalConstKey.TypeAllocate, OnAlloCateCds);

            TrunBackAllDipai();
        }

        /// <summary>
        /// 游戏信息缓存
        /// </summary>
        private ISFSObject _gameInfoTemp;
        private int[] _dPaicdsTemp;


        [SerializeField] protected UILabel AnteLabel;
        /// <summary>
        /// 临时存储倍数
        /// </summary>
        private int _beishu = 1;
        [SerializeField] protected UILabel BeiShuLabel;
        [SerializeField] protected UILabel RoundLabel;
        [SerializeField] protected UILabel RoomIdLabel;

        /// <summary>
        /// 地主获得的底牌
        /// </summary>
        [SerializeField] protected DipaicdItem[] DpaiCds;
         
        /// <summary>
        /// 底牌的动画
        /// </summary>
        [SerializeField] protected GameObject[] DipaiCdsAnims;

        /// <summary>
        /// 底牌的grid
        /// </summary>
        [SerializeField] protected UIGrid DpGrid;
        /// <summary>
        /// 倍数与局数的Grid，用来控制显示布局 
        /// </summary>
        [SerializeField] protected UIGrid BjGrid;
        /// <summary>
        /// 局数的Obj，娱乐模式中不显示局数
        /// </summary>
        [SerializeField] protected GameObject RoundObj;
        /// <summary>
        /// 房间号Obj,娱乐模式中不显示房间号
        /// </summary>
        [SerializeField] protected GameObject RoomIdObj;
        /// <summary>
        /// gameinfo
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        protected void OnGameInfo(object sender, DdzbaseEventArgs args)
        {
            SetGameInfo(sender, args);
            //游戏人没齐的时候，局数是0 所以要变成1局
            if (_gameInfoTemp.ContainsKey(NewRequestKey.KeyCurRound) && _gameInfoTemp.ContainsKey(NewRequestKey.KeyMaxRound))
            {
                if (_gameInfoTemp.GetInt(NewRequestKey.KeyCurRound) == 0)
                {
                    RoundLabel.text = 1 + "/" +
                                 _gameInfoTemp.GetInt(NewRequestKey.KeyMaxRound);
                }
            }
            bool isRoomType = App.GetGameData<GlobalData>().IsRoomGame;
            if (RoomIdObj)
            {
               RoomIdObj.SetActive(isRoomType);
            }
            if (RoundObj)
            {
                RoundObj.SetActive(isRoomType);
                if (BjGrid)
                {
                    BjGrid.repositionNow = true;
                }
            }
        }

        protected void OnRejoinGame(object sender, DdzbaseEventArgs args)
        {
            SetGameInfo(sender, args);

            bool isRoomType = App.GetGameData<GlobalData>().IsRoomGame;
            if (RoomIdObj)
            {
                RoomIdObj.SetActive(isRoomType);
            }
            if (RoundObj)
            {
                RoundObj.SetActive(isRoomType);
                if (BjGrid)
                {
                    BjGrid.repositionNow = true;
                }
            }
        }

       
        private void SetGameInfo(object sender,DdzbaseEventArgs args)
        {
            var data = args.IsfObjData;
            if(data==null) throw new Exception("得到了空的服务器信息");
            _gameInfoTemp = data;
            RefreshUiInfo();
        }

        public override void RefreshUiInfo()
        {
            if (_gameInfoTemp.ContainsKey(NewRequestKey.KeyAnte)) 
                AnteLabel.text = _gameInfoTemp.GetInt(NewRequestKey.KeyAnte).ToString(CultureInfo.InvariantCulture);

            if (_gameInfoTemp.ContainsKey(RequestKey.KeyScore))
            {

                var score = (_gameInfoTemp.GetInt(RequestKey.KeyScore));
                var user = _gameInfoTemp.GetSFSObject(RequestKey.KeyUser);
                var ttscore = score;
                if (user.ContainsKey(NewRequestKey.KeyRate))
                {
                    ttscore = score*user.GetInt(NewRequestKey.KeyRate);
                }
                _beishu = ttscore;
                BeiShuLabel.text = _beishu.ToString(CultureInfo.InvariantCulture);
            }

            if (_gameInfoTemp.ContainsKey(NewRequestKey.KeyMaxRound) &&
                _gameInfoTemp.ContainsKey(NewRequestKey.KeyCurRound))
            {
                RoundLabel.text = _gameInfoTemp.GetInt(NewRequestKey.KeyCurRound) + "/" +
                           _gameInfoTemp.GetInt(NewRequestKey.KeyMaxRound);
            }

            if (_gameInfoTemp.ContainsKey(NewRequestKey.KeyRoomId)) 
                RoomIdLabel.text = _gameInfoTemp.GetInt(NewRequestKey.KeyRoomId).ToString(CultureInfo.InvariantCulture);

            if (_gameInfoTemp.ContainsKey(NewRequestKey.KeyLandCds))
                _dPaicdsTemp = _gameInfoTemp.GetIntArray("landCards");
              
            SetDpaiCds();
        }


        /// <summary>
        /// 当收到服务TypeFirstOut器相应
        /// </summary>
        private void TypeFirstOut(object sender, DdzbaseEventArgs args)
        {
            var data = args.IsfObjData;
            //更新局数信息缓存
            _gameInfoTemp.PutInt(NewRequestKey.KeyCurRound, _gameInfoTemp.GetInt(NewRequestKey.KeyCurRound) + 1);
            _dPaicdsTemp = data.GetIntArray(RequestKey.KeyCards);

            //开始翻牌动画
            StopCoroutine("ShowDipaiCdsTrunAnim");
            StartCoroutine("ShowDipaiCdsTrunAnim");

            //如果没有叫倍数，默认倍数为1
            BeiShuLabel.text = _beishu.ToString(CultureInfo.InvariantCulture);

            /*            try
                        {
                            var curbeishu = int.Parse(BeiShuLabel.text);
                            if (curbeishu < 1) BeiShuLabel.text = "1";
                        }
                        catch (Exception e)
                        {
                            YxDebug.LogError("BeiShuLabel有问题："+ e.Message);
                            BeiShuLabel.text = "1";
                        }*/
        }

        /// <summary>
        /// 动画以后显示底牌
        /// </summary>
        private IEnumerator ShowDipaiCdsTrunAnim()
        {

            foreach (var dpcd in DpaiCds)
            {
                dpcd.gameObject.SetActive(false);
            }

            if (DipaiCdsAnims != null)
            {
                foreach (var dpcdanim in DipaiCdsAnims)
                {
                    dpcdanim.SetActive(true);
                    var spanim = dpcdanim.GetComponent<UISpriteAnimation>();
                    if(spanim==null)continue;

                    spanim.ResetToBeginning();
                    spanim.Play();
                }

                var lastCdAnim = DipaiCdsAnims[DipaiCdsAnims.Length - 1].GetComponent<UISpriteAnimation>();
                //检测播放
                while (lastCdAnim.isPlaying)
                {
                    yield return new WaitForEndOfFrame();
                }

            }

            //设置底牌数值
            SetDpaiCds();
        }

        /// <summary>
        /// 当玩家准备时
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        protected void OnUserReady(object sender, DdzbaseEventArgs args)
        {
            TrunBackAllDipai();
            _beishu = 1;
            BeiShuLabel.text = "0";
        }

        /// <summary>
        /// 当底注改变时候
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        protected void OnChangDizhu(object sender, DdzbaseEventArgs args)
        {
            var data = args.IsfObjData;

            if (data.ContainsKey("ante"))
            {
                int ante = data.GetInt("ante");
                AnteLabel.text = ante.ToString(CultureInfo.InvariantCulture);
            }
            if (data.ContainsKey("rate"))
            {
                int rate = data.GetInt("rate");
                if (rate > 1)
                {
                    StartCoroutine(ChangeTextSize());
                }
            }
        }

        /// <summary>
        /// 抢地主时
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void OnTypeGrab(object sender, DdzbaseEventArgs args)
        {
            var data = args.IsfObjData;
            var score = data.GetInt(RequestKey.KeyScore);
            _gameInfoTemp.PutInt(RequestKey.KeyScore,score);

            //如果有人叫分小于等于之前叫分，忽略之
            if (_beishu >= score) return;

            _beishu = score;
            BeiShuLabel.text = _beishu.ToString(CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// 有人出牌时,检查是否有炸弹火箭等，从而显示加倍
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void OnTypeOutCard(object sender, DdzbaseEventArgs args)
        {
            var data = args.IsfObjData;
            var cards = data.GetIntArray(RequestKey.KeyCards);
            cards = HdCdsCtrl.SortCds(cards);
            var cdsType = PokerRuleUtil.GetCdsType(cards);
            if (cdsType == CardType.C4 || cdsType == CardType.C42 || cdsType == CardType.C5)
            {
                try
                {
                    //var curbeishu = int.Parse(BeiShuLabel.text);
                    _beishu *= 2;
                    BeiShuLabel.text = _beishu.ToString(CultureInfo.InvariantCulture);
                }
                catch (Exception e)
                {
                    YxDebug.LogError("BeiShuLabel有问题：" + e.Message);
                }
            }
        }


        /// <summary>
        /// 当收到加倍已经结束的信息
        /// </summary>
        private void OnDoubleOver(object sender, DdzbaseEventArgs args)
        {
            var data = args.IsfObjData;

            var rates = data.GetIntArray("jiabei");
            var len = rates.Length;
            var selfSeat = App.GetGameData<GlobalData>().GetSelfSeat;
            for (int i = 0; i < len; i++)
            {
                if (i != selfSeat) continue;
                _beishu = (_gameInfoTemp.GetInt(RequestKey.KeyScore)*rates[i]);
                BeiShuLabel.text = _beishu.ToString(CultureInfo.InvariantCulture);
            }
        }

        /// <summary>
        /// 设置底牌
        /// </summary>
        void SetDpaiCds()
        {
            //隐藏翻牌动画
            if (DipaiCdsAnims != null)
            {
                foreach (var dpcdanim in DipaiCdsAnims)
                {
                    dpcdanim.SetActive(false);
                }
            }

            if (_dPaicdsTemp == null || _dPaicdsTemp.Length <1 || _dPaicdsTemp.Length>DpaiCds.Length)
            {
                return;
            }
            var dpvalueLen = _dPaicdsTemp.Length;

            var dpGobLen = DpaiCds.Length;
            for (int i = 0; i < dpGobLen; i++)
            {
                if (i >= dpvalueLen)
                {
                    DpaiCds[i].gameObject.SetActive(false);
                    continue;
                }

                DpaiCds[i].SetLayer(DpaiCds[i].GetComponent<UISprite>().depth);
                DpaiCds[i].SetDipaiValue(_dPaicdsTemp[i]);
            }
            DpGrid.repositionNow = true;
        }

        /// <summary>
        /// 发牌时，计算局数
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        void OnAlloCateCds(object sender, DdzbaseEventArgs args)
        {
            //更新局数label
            if (_gameInfoTemp.ContainsKey(NewRequestKey.KeyMaxRound) &&
                _gameInfoTemp.ContainsKey(NewRequestKey.KeyCurRound))
                RoundLabel.text = _gameInfoTemp.GetInt(NewRequestKey.KeyCurRound)+1 + "/" +
                                  _gameInfoTemp.GetInt(NewRequestKey.KeyMaxRound);
        }

        /// <summary>
        /// 把所有底牌背过去
        /// </summary>
        private void TrunBackAllDipai()
        {
            if (DpaiCds == null) return;
            foreach (var dpItem in DpaiCds)
            {
                dpItem.TrunBackCd();
            }
        }


        private IEnumerator ChangeTextSize()
        {
            yield return new WaitForSeconds(3);
            int times = 0;
            while (times < 3)
            {
                times++;
                int size = 40;
                AnteLabel.fontSize = size;
                yield return 1;
                while (size > 25)
                {
                    size--;
                    AnteLabel.fontSize = size;
                    yield return 1;
                }
                yield return new WaitForSeconds(1);
            }
        }
    }
}
