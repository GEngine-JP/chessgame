using System;
using System.Collections;
using System.Collections.Generic;
using com.yxixia.utile.YxDebug;
using UnityEngine;
using YxFramwork.Common;
using YxFramwork.Framework.Core;
using YxFramwork.Manager;

namespace Assets.Scripts.Game.rbwar
{
    public class TipCtrl : MonoBehaviour
    {
        public GameObject StartBetBg;

        public GameObject StopBetBg;

        public GameObject StopBlackBg;

        public GameObject CompareCard;

        public GameObject CompareMan;

        public GameObject CompareWoman;

        public GameObject PleaseBet;

        public UISprite TimeFirst;
        public UISprite TimeSecond;

        public GameObject Waiting;

        public List<TweenAlpha> ResultShowList = new List<TweenAlpha>();

        private int _timeCd;

        private RbwarGameData _gdata
        {
            get { return App.GetGameData<RbwarGameData>(); }
        }

        private RbwarGameManager _gmanager
        {
            get { return App.GetGameManager<RbwarGameManager>(); }
        }

        public void StartBetTip()
        {
            StartBetBg.transform.localPosition = new Vector3(-1450, 27, 0);
            Facade.Instance<MusicManager>().Play("show");
            TweenPosition.Begin(StartBetBg, _gdata.UnitTime*4, new Vector3(0, 27, 0));
            StartBetBg.GetComponent<TweenPosition>().onFinished.Clear();
            Facade.Instance<MusicManager>().Play("start");
            StartBetBg.GetComponent<TweenPosition>().AddOnFinished(() =>
            {
                StartBetBg.GetComponent<TweenPosition>().delay = _gdata.UnitTime*5;
               
                TweenPosition.Begin(StartBetBg, _gdata.UnitTime*4, new Vector3(1450, 27, 0)); 
            });
        }

        public void StopBetTip()
        {
            StopBetBg.transform.localPosition = new Vector3(-1450, 27, 0);
            Facade.Instance<MusicManager>().Play("show");

            StopBlackBg.GetComponent<UITexture>().color = new Color(0, 0, 0, 1);

            TweenPosition.Begin(StopBetBg, _gdata.UnitTime*4, new Vector3(0, 27, 0));

              Facade.Instance<MusicManager>().Play("stop");
            StopBetBg.GetComponent<TweenPosition>().AddOnFinished(() =>
            {
                StopBetBg.GetComponent<TweenPosition>().delay = _gdata.UnitTime*5;
              
                TweenPosition.Begin(StopBetBg, _gdata.UnitTime*4, new Vector3(1450, 27, 0));

                TweenAlpha.Begin(StopBlackBg, _gdata.UnitTime*2, 0);
                StopBlackBg.GetComponent<TweenAlpha>().delay = _gdata.UnitTime * 3;
                StopBlackBg.GetComponent<TweenAlpha>().AddOnFinished(() =>
                {
                    StopBetBg.GetComponent<TweenPosition>().onFinished.Clear();
                });
            });
        }

        public void CompareCardTip()
        {
            CompareCard.SetActive(true);
            TweenPosition.Begin(CompareMan, _gdata.UnitTime*3, new Vector3(-235, 75, 0));


            TweenPosition.Begin(CompareWoman, _gdata.UnitTime*3, new Vector3(235, 85, 0));
            CompareWoman.GetComponent<TweenPosition>().onFinished.Clear();

            CompareWoman.GetComponent<TweenPosition>().AddOnFinished(() =>
            {
                CompareMan.GetComponent<TweenPosition>().delay = _gdata.UnitTime*5;
                TweenPosition.Begin(CompareMan, _gdata.UnitTime*4, new Vector3(-1000, 75, 0));

                CompareWoman.GetComponent<TweenPosition>().delay = _gdata.UnitTime*5;
                TweenPosition.Begin(CompareWoman, _gdata.UnitTime*4, new Vector3(1000, 85, 0));

                CompareWoman.GetComponent<TweenPosition>().AddOnFinished(() =>
                {
                    CompareCard.SetActive(false);
                    StartBetTip();
                });

            });
        }

        public void BetTime(int time)
        {
            PleaseBet.SetActive(true);
            _timeCd = time;
            InvokeRepeating("TimeChange", 0, 1);
        }

        private void TimeChange()
        {
            if (_timeCd >= 10)
            {
                var timeS = _timeCd / 10 % 10;
                TimeFirst.gameObject.SetActive(true);
                TimeFirst.spriteName =string.Format("num{0}", timeS);
                var timeG = _timeCd % 10;
                TimeSecond.gameObject.SetActive(true);
                TimeSecond.spriteName =string.Format("num{0}", timeG);
            }
            else
            {
                if (_timeCd == 0)
                {
                    PleaseBet.gameObject.SetActive(false);

                    TimeFirst.gameObject.SetActive(false);
                    CancelInvoke("TimeChange");
                    Facade.Instance<MusicManager>().Play("alert");
                }
                else
                {
                    if (_timeCd <= 5)
                    {
                        Facade.Instance<MusicManager>().Play("countdown");
                    }
                    TimeSecond.gameObject.SetActive(false);
                    TimeFirst.spriteName =string.Format("num{0}", _timeCd);
                }
            }

            _timeCd--;
        }

        public void ShowWaiting()
        {
            Waiting.SetActive(true);
        }

        public void Result(List<int> list)
        {
            _gmanager.LaterSend = true;
            StopAllCoroutines();

            StartCoroutine(StopResultShow(list));
        }

        private IEnumerator StopResultShow(List<int> list)
        {
            foreach (var t in list)
            {
                ResultShowList[t].gameObject.SetActive(true);
                ResultShowList[t].enabled = true;
            }

            yield return new WaitForSeconds(_gdata.UnitTime*12);

            foreach (var t in list)
            {
                ResultShowList[t].enabled = false;
                ResultShowList[t].gameObject.SetActive(false);
            }
            yield return new WaitForSeconds(_gdata.UnitTime *2);

            ShowUserWin();
            _gmanager.LaterSend = false;
        }

        private void ShowUserWin()
        {
            var betCtrl = _gmanager.BetCtrl;

            var count = Math.Min(_gmanager.CurrentTableCount, 6);

            var selfPos = 0;

            for (int i = 0; i < count; i++)
            {
                for (int j = 0; j < _gdata.AllUserInfos.Count; j++)
                {
                    if (_gdata.AllUserInfos[j].Seat == _gdata.GoldRank[i])
                    {
                        if (_gdata.AllUserInfos[j].Seat == App.GameData.SelfSeat)
                        {
                            _gdata.GetPlayer<RbwarPlayer>().ShowResult(_gdata.AllUserInfos[j].WinCoin, _gdata.AllUserInfos[j].CoinA);
//                            selfPos = 6;
                        }
                        //                        else
                        //                        {
                        _gmanager.SpecialPlayers[i].ShowResult(_gdata.AllUserInfos[j].WinCoin, _gdata.AllUserInfos[j].CoinA);
                            selfPos = i;
//                        }

                        if (_gdata.AllUserInfos[j].WinCoin > 0) 
                        {
                            var sd = betCtrl._chipArea.childCount * _gdata.AllUserInfos[j].WinCoin * 1.0f / _gdata.AllUserWinGolds;

                            var dd = Math.Floor(sd);

                            //                            Debug.LogError("座位号"+i+"需要移动筹码数"+ dd);

                            _gmanager.BetCtrl.ChipMoveBack(selfPos, (int)dd);
                        }


                        _gmanager.SpecialPlayers[i].Info = _gdata.AllUserInfos[j];
                    }
                    else
                    {
                        if (_gdata.AllUserInfos[j].Seat == App.GameData.SelfSeat)//判断当是自己不在排行中的时候 界面的显示
                        {
                            _gdata.GetPlayer<RbwarPlayer>().ShowResult(_gdata.AllUserInfos[j].WinCoin, _gdata.AllUserInfos[j].CoinA);
                        }

                        if (i == count - 1 && j == _gdata.AllUserInfos.Count - 1)
                        {
                            _gmanager.BetCtrl.ChipMoveBack(-1, -1);
                        }
                    }
                }
            }
        
        }

        public void Clear()
        {
            CompareCard.SetActive(false);
            Waiting.SetActive(false);
            PleaseBet.SetActive(false);
        }
    }
}
