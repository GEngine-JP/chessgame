using System;
using System.Collections.Generic;
using Assets.Scripts.Common.components;
using Sfs2X.Entities.Data;
using UnityEngine;
using YxFramwork.Common;
using YxFramwork.Enums;
using YxFramwork.Framework;
using YxFramwork.Framework.Core;
using YxFramwork.Manager;
using YxFramwork.Tool;
using YxFramwork.View;
using Random = System.Random;

namespace Assets.Scripts.Game.rbwar
{
    public class BetCtrl : YxView
    {

        public UILabel RedSelfBetLabel;
        public UILabel BlackSelfBetLabel;
        public UILabel LuckySelfBetLabel;

        public GameObject Select;
        /// <summary>
        /// 筹码区域
        /// </summary>
        public GameObject ChipParents;
        public ResetChip AllBet;

        public List<UILabel> BetGoldList=new List<UILabel>();

        public Chip CurrentSelectChip;
        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        public ChipConfig ChipCfg = new ChipConfig();

        private int[] _betGold={0,0,0};
        private int[] _selfGold = { 0,0,0};

        private RbwarGameData _gdata
        {
            get { return App.GetGameData<RbwarGameData>(); }
        }

        private RbwarGameServer _gserver
        {
            get { return App.GetRServer<RbwarGameServer>(); }
        }

        private RbwarGameManager _gmanager
        {
            get { return App.GetGameManager<RbwarGameManager>(); }
        }

        private void FreshCurrentUserBet(int pos,int gold)
        {
            switch (pos)
            {
                case 0:
                    if (!RedSelfBetLabel.gameObject.activeSelf)
                    {
                        RedSelfBetLabel.gameObject.SetActive(true);
                    }
                    _selfGold[0] += gold;
                    RedSelfBetLabel.text = YxUtiles.ReduceNumber(_selfGold[0]);
                    break;
                case 1:
                    if (!BlackSelfBetLabel.gameObject.activeSelf)
                    {
                        BlackSelfBetLabel.gameObject.SetActive(true);
                    }
                    _selfGold[1] += gold;
                    BlackSelfBetLabel.text = YxUtiles.ReduceNumber(_selfGold[1]);
                    break;
                case 2:
                    if (!LuckySelfBetLabel.gameObject.activeSelf)
                    {
                        LuckySelfBetLabel.gameObject.SetActive(true);
                    }
                    _selfGold[2] += gold;
                    LuckySelfBetLabel.text = YxUtiles.ReduceNumber(_selfGold[2]);
                    break;
            }
        }

      public void OnBetClick(Chip chip)
        {
            Select.transform.position = chip.transform.position;
            CurrentSelectChip = chip;
        }
      
        protected override void OnStart()
        {
            Select.SetActive(false);
            Init();
        }

        public virtual void OnDeskClick(UIWidget widget)
        {
            string index= widget.name;
            if (CurrentSelectChip == null) { return; }
            if (!_gdata.BeginBet) { return; }
            var chipData = CurrentSelectChip.GetData<ChipData>();
            if (chipData == null) { return; }
            var gold = chipData.Value;
            if (gold > _gdata.GetPlayerInfo().CoinA)
            {
                YxMessageBox.Show(new YxMessageBoxData { Msg = "金币不够，无法下注." });
                return;
            }
            _gserver.UserBet(index, gold);

//            var ds=Facade.Instance<MusicManager>().Play("Bet").length;

//            var lpos = CurrentSelectChip.transform.position;
//            lpos = _chipArea.InverseTransformPoint(lpos);
//            InstantiateChip(widget, lpos, gold);
        }


        public void ShowChip()
        {
            if (!_gdata.BeginBet) return;
            AllBet.gameObject.SetActive(true);
            Select.SetActive(true);

        }
        public void HideChip()
        {
            AllBet.gameObject.SetActive(false);
            Select.SetActive(false);

            for (int i = 0; i < _betGold.Length; i++)
            {
                _betGold[i] = 0;
                _selfGold[i] = 0;
            }
        }
        public virtual void Bet(ISFSObject responseData)
        {
            var seat = responseData.GetInt("seat");
            var gold = responseData.GetInt("gold");

            var p = responseData.GetUtfString("p");
            var index = FindBetP(p);

            _betGold[index] += gold;
            BetGoldList[index].text = YxUtiles.ReduceNumber(_betGold[index]);

            Facade.Instance<MusicManager>().Play("bet");

            if (seat == _gdata.SelfSeat)
            {
                _gdata.GetPlayer().Coin -= gold;
                FreshCurrentUserBet(index, gold);
                App.GameData.GStatus = YxEGameStatus.PlayAndConfine;
//                return;
            }
            //其他人
            var startPos = ChipCfg.StartPos;
            var len = startPos.Length;

            if (len > 0)
            {
                var pos = 0;
                for (int i = 0; i < _gmanager.CurrentTableCount; i++)
                {
                    if (_gdata.GoldRank[i] == seat)
                    {
                        if (i == 0)
                        {
                            _gmanager.SpecialPlayers[i].ShowStarMove(index);
                        }
                        _gmanager.SpecialPlayers[i].Coin -= gold;
                        if (seat != _gdata.SelfSeat)
                        {
                            var parent = _gmanager.SpecialPlayers[i].HeadPortrait.transform.GetComponentInParent<TweenPosition>();
                            if (parent == null) return;
                            if (parent.onFinished != null)
                            {
                                parent.onFinished.Clear();
                            }
                            parent.PlayForward();
                            parent.AddOnFinished(() =>
                            {
                                parent.PlayReverse();
                            });
                        }
                        pos = i + 1;
                        break;
                    }
                }

                if (seat != _gdata.SelfSeat)
                {
                    InstantiateChip(ChipCfg.DeskAreas[index], startPos[pos].localPosition, gold);
                }
                else
                {
                    var lpos = CurrentSelectChip.transform.position;
                    lpos = _chipArea.InverseTransformPoint(lpos);
                    InstantiateChip(ChipCfg.DeskAreas[index], lpos, gold);
                }

                //                var random = UnityEngine.Random.Range(0, 100) % len;
                //                InstantiateChip(ChipCfg.DeskAreas[index], startPos[random], gold);
            }
        }

        private int FindBetP(string posName)
        {
            var index = -1;
            switch (posName)
            {
                case "red":
                    index = 0;
                    break;
                case "black":
                    index = 1;
                    break;
                case "luck":
                    index = 2;
                    break;
            }

            return index;
        }
        private int _chipdepth = 1;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="widget"></param>
        /// <param name="localPos"></param>
        /// <param name="gold"></param>
        /// <param name="needAnimo"></param>
        public void InstantiateChip(UIWidget widget, Vector3 localPos, int gold, bool needAnimo = true)
        {
            var chip = Instantiate(ChipCfg.ChipPerfab);
            var chipTs = chip.transform;
            chipTs.parent = _chipArea;
            chipTs.localPosition = localPos;
            chipTs.localScale = new Vector3(0.6f, 0.6f, 0.6f);
            _chipdepth += 2;
            var data = new ChipData
            {
                Value = gold,
                BgId = _gdata.AnteRate.IndexOf(gold),
                Depth = _chipdepth
            };
            chip.UpdateView(data);
            chip.gameObject.SetActive(true);
            if (!needAnimo) { return; }

            _allVector3S.Add(GetClipPos(widget));
            TweenPosition.Begin(chip.gameObject, _gdata.UnitTime*1.5f, GetClipPos(widget));
            var sp = chip.GetComponent<TweenPosition>();
            sp.delay = _gdata.UnitTime;
//            sp.target = GetClipPos(widget);
//            sp.enabled = true;
            sp.AddOnFinished(() =>
            {
                TweenScale.Begin(sp.gameObject, _gdata.UnitTime*3, new Vector3(0.5f, 0.5f, 0.5f));
                sp.GetComponent<TweenScale>().delay = _gdata.UnitTime*2;
            });}

        public Vector3 GetClipPos(UIWidget widget)
        {
            var v = Vector3.zero;
            var r = new Random();
            var w = widget.width / 4;
            var h = widget.height / 4;
            var i2 = r.Next(-w, w);
            var i3 = r.Next(-h, h);
            var ts = widget.transform;
            v.x = ts.localPosition.x + i2;
            v.y = ts.localPosition.y + i3;
            return v;
        }

        public void InitChips()
        {
            OnBetClick(AllBet.InitChips(_gdata.Ante, _gdata.AnteRate));
        }
        public Transform _chipArea;

        public override void Init()
        {
            _chipdepth = 1;

            _index = 0;
            _index1 = 0;
            _residue = 0;

            foreach (var betGold in BetGoldList)
            {
                betGold.text = "";
            }

            RedSelfBetLabel.gameObject.SetActive(false);
            BlackSelfBetLabel.gameObject.SetActive(false);
            LuckySelfBetLabel.gameObject.SetActive(false);

            InitChipArea();
            _allVector3S.Clear();
        }

        private void InitChipArea()
        {
            if (_chipArea != null)
            {
                Destroy(_chipArea.gameObject);
            }
            var go = new GameObject("ChipArea");
            _chipArea = go.transform;
            _chipArea.parent = ChipParents.transform;
            _chipArea.localPosition = Vector3.zero;
            _chipArea.localScale = Vector3.one;
            _chipArea.localRotation = Quaternion.identity;
        }

        private readonly List<Vector3> _allVector3S = new List<Vector3>();

        private int _index;
        private int _index1;

        private int _residue;


        public void ChipMoveBack(int pos=0,int num = -1)
        {
            
            Facade.Instance<MusicManager>().Play("win_bet");  
            if (num ==-1)
            {
                num = _residue==0? _chipArea.transform.childCount - _index1: _residue;
            }
            _index += num;

//                        Debug.LogError("当前的筹码子物体的个数" + _chipArea.transform.childCount);
//                        StartCoroutine(Move(pos, num, count));
//                        Debug.LogError("当前的起始位置" + _index1 + "当前的个数" + _index);
            if (_index > _chipArea.transform.childCount)
            {
                return;
            }
            for (int i = _index1; i < _index; i++)
            {
                if (_chipArea.transform.GetChild(i).GetComponent<TweenScale>() != null)
                {   
                    _chipArea.transform.GetChild(i).GetComponent<TweenScale>().PlayReverse();
                }
                var item = _chipArea.transform.GetChild(i).GetComponent<TweenPosition>();

                item.ResetToBeginning();
                item.transform.localPosition = _allVector3S[i];
                item.delay =_gdata.UnitTime*2;

                item.duration = _gdata.UnitTime*7;
                item.from = _allVector3S[i];
                item.to = ChipCfg.StartPos[pos+1].localPosition;
                item.enabled = true;

                var i1 = i;
                item.AddOnFinished(() =>
                {
                    _chipArea.transform.GetChild(i1).gameObject.SetActive(false);
                });
            }
            _index1 += num;
           _residue = _chipArea.transform.childCount - _index1;
        }


        [Serializable]
        public class ChipConfig
        {
            /// <summary>
            /// 
            /// </summary>
            public Chip ChipPerfab;
            public Transform[] StartPos;
            public UIWidget[] DeskAreas;
        }
    }

}
