using System.Collections.Generic;
using YxFramwork.ConstDefine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine;


namespace Assets.Scripts.Game.Mahjong3D.Standard
{
    [UIPanelData(typeof(PanelHuanAndDq), UIPanelhierarchy.Base)]
    public class PanelHuanAndDq : UIPanelBase
    {
        public TextTipItem TipText;
        public Transform ChangeTitle;
        public ChangeCardTip ChangeTip;

        public DqBtnItem[] DqBtns;
        public ObjContainer HusContainer;

        public Dictionary<int, List<EffectObject>> mDingquEffectCache = new Dictionary<int, List<EffectObject>>();
        private Image[] mHuGroup;

        private void Awake()
        {
            var Event = GameCenter.EventHandle;
            Event.Subscriber((int)UIEventProtocol.SetPlayerFlagState, SetPlayerFlagState);
            Event.Subscriber((int)UIEventProtocol.HideChangeTitleBtn, HidenConfirmBtn);
            Event.Subscriber((int)UIEventProtocol.TipBankerPutCard, TipBankerPutCard);
            Event.Subscriber((int)UIEventProtocol.ShowDingqueFlag, ShowDingqueFlag);
            Event.Subscriber((int)UIEventProtocol.SetSingleHuFlag, SetSingleHuFlag);
            Event.Subscriber((int)UIEventProtocol.ChangeCardTip, ChangeCardTip);
        }

        public override void OnContinueGameUpdate()
        {
            mDingquEffectCache.Clear();
        }

        public override void OnGetInfoUpdate()
        {
            mHuGroup = HusContainer.GetComponent<Image>(DataCenter.MaxPlayerCount);
            for (int i = 0; i < mHuGroup.Length; i++)
            {
                mHuGroup[i].gameObject.SetActive(false);
            }

            //隐藏UI
            HideDqBtns();
            TipText.gameObject.SetActive(false);
            ChangeTip.gameObject.SetActive(false);
            ChangeTitle.gameObject.SetActive(false);
        }

        public void ChangeCardTip(EvtHandlerArgs args)
        {
            var param = args as HuanAndDqArgs;
            ChangeTip.ShowTip(param.HuanType);
        }

        public void TipBankerPutCard(EvtHandlerArgs args)
        {
            StartCoroutine(IeTipAnimation());
        }

        private IEnumerator IeTipAnimation()
        {
            TipText.ExCompShow().Content = "您是庄家 请出牌！";
            yield return new WaitForSeconds(2);
            TipText.ExCompHide();
        }

        public override void OnReconnectedUpdate()
        {
            MahjongUserInfo data;
            for (int i = 0; i < DataCenter.MaxPlayerCount; i++)
            {
                data = DataCenter.Players[i];
                if (null != data)
                {
                    int huanType = data.ExtData.Get<VarInt>("htype");
                    if (huanType != 0)
                    {
                        int chair = data.Chair;
                        SetPlayerDingqueFlag(chair, huanType);
                    }
                    int state = data.ExtData.Get<VarInt>("state");
                    if (state == (int)XzmjGameStatue.hu && MahjongUtility.GameKey == GameMisc.XzmjKey)
                    {
                        mHuGroup[i].gameObject.SetActive(true);
                    }
                }
            }
        }

        public override void OnReadyUpdate()
        {
            HideHuFlag();
            HideDingqueFlag();
        }

        public void SetSingleHuFlag(EvtHandlerArgs args)
        {
            var param = args as HuanAndDqArgs;
            if (param.Type == 2)
            {
                int chair;
                var seats = param.HuSeats;
                for (int i = 0; i < seats.Count; i++)
                {
                    chair = MahjongUtility.GetChair(seats[i]);
                    mHuGroup[chair].gameObject.SetActive(true);
                    mHuGroup[chair].GetComponent<TweenPosition>().Do((cmp) =>
                    {
                        cmp.ResetToBeginning();
                        cmp.PlayForward();
                    });
                }
            }
        }

        private void HideHuFlag()
        {
            if (mHuGroup == null) return;
            for (int i = 0; i < mHuGroup.Length; i++)
            {
                mHuGroup[i].gameObject.SetActive(false);
            }
        }

        public void ShowDingqueFlag(EvtHandlerArgs args)
        {
            var param = args as HuanAndDqArgs;
            if (param.Type == 0)
            {
                HideDingqueFlag();
                var colors = param.DingqueColors;
                MahjongUtility.PlayEnvironmentSound("feidingque");
                for (int i = 0; i < colors.Length; i++)
                {
                    int chair = MahjongUtility.GetChair(i);
                    SetPlayerDingqueFlag(chair, colors[i]);
                }
            }
        }

        private void SetPlayerDingqueFlag(int chair, int color)
        {
            PoolObjectType effectType = PoolObjectType.none;
            EffectObject effect = null;
            List<EffectObject> list;
            if (0x10 == color)
            {
                effectType = PoolObjectType.dqwan;
            }
            else if (0x20 == color)
            {
                effectType = PoolObjectType.dqtiao;
            }
            else if (0x30 == color)
            {
                effectType = PoolObjectType.dqtong;
            }
            if (mDingquEffectCache.TryGetValue(chair, out list))
            {
                for (int i = 0; i < list.Count; i++)
                {
                    if (list[i].Type == effectType)
                    {
                        effect = list[i];
                    }
                }
                if (effect == null)
                {
                    effect = CreateDingqueEffect(mDingquEffectCache[chair], chair, effectType);
                }
            }
            else
            {
                mDingquEffectCache[chair] = new List<EffectObject>();
                effect = CreateDingqueEffect(mDingquEffectCache[chair], chair, effectType);
            }
            effect.gameObject.SetActive(true);
            effect.Execute();
        }

        private EffectObject CreateDingqueEffect(IList<EffectObject> list, int chair, PoolObjectType effectType)
        {
            EffectObject effect = MahjongUtility.PlayMahjongEffect(effectType);
            mDingquEffectCache[chair].Add(effect);
            var item = MahjongUtility.GetYxGameData().GetPlayerInfoItem<PlayerInfoItem>(chair);
            if (null != item)
            {
                var iamge = item.Owner.ExCompShow().GetComponent<Image>();
                iamge.enabled = false;
                effect.ExSetParent(iamge.transform);
            }
            return effect;
        }

        private void HideDingqueFlag()
        {
            var e = mDingquEffectCache.GetEnumerator();
            while (e.MoveNext())
            {
                var list = e.Current.Value;
                for (int i = 0; i < list.Count; i++)
                {
                    if (list[i] != null)
                    {
                        list[i].gameObject.SetActive(false);
                    }
                }
            }
        }

        private void ShowDqBtns(int color)
        {
            int value = -1;
            switch (color)
            {
                case 0x10: value = 0; break;
                case 0x20: value = 1; break;
                case 0x30: value = 2; break;
            }
            for (int i = 0; i < DqBtns.Length; i++)
            {
                if (i == value)
                {
                    DqBtns[i].ShowEffect();
                }
                else
                {
                    DqBtns[i].gameObject.SetActive(true);
                }
            }
        }

        private void HideDqBtns()
        {
            for (int i = 0; i < DqBtns.Length; i++)
            {
                DqBtns[i].gameObject.SetActive(false);
            }
        }

        public void SetPlayerFlagState(EvtHandlerArgs args)
        {
            var param = args as PlayerStateFlagArgs;
            if (param.CtrlState)
            {
                switch (param.StateFlagType)
                {
                    case (int)PlayerStateFlagType.Selecting:
                        TipText.ExCompShow().Content = "缺一门 才能胡！";
                        ShowDqBtns(param.SecletColor);
                        break;
                    case (int)PlayerStateFlagType.SelectCard:
                        {
                            ChangeTitle.gameObject.SetActive(true);
                            var text = ChangeTitle.transform.FindChild("title").GetComponent<Text>();
                            text.text = "选择以下3张{0}手牌！".ExFormat("<color=#FFE200FF>同花色</color>");
                        }
                        break;
                }
            }
            else
            {
                ChangeTitle.gameObject.SetActive(false);
                HideDqBtns();
                TipText.ExCompHide();
            }
        }

        /// <summary>
        /// 选牌确定
        /// </summary>
        public void OnConfirmClick()
        {
            GameCenter.Network.C2S.Custom<C2SCustom>().OnConfirmChangeClick();
        }

        public void HidenConfirmBtn(EvtHandlerArgs args)
        {
            ChangeTitle.gameObject.SetActive(false);
        }

        /// <summary>
        /// 定缺：万
        /// </summary>
        public void OnWanClick()
        {
            GameCenter.Network.OnRequestC2S((sfs) =>
            {
                sfs.PutInt(RequestKey.KeyType, NetworkProtocol.MJSelectColor);
                sfs.PutInt("color", 0x10);
                return sfs;
            });
            HideDqBtns();
            TipText.ExCompHide();
        }

        /// <summary>
        /// 定缺：条
        /// </summary>
        public void OnTiaoClick()
        {
            GameCenter.Network.OnRequestC2S((sfs) =>
            {
                sfs.PutInt(RequestKey.KeyType, NetworkProtocol.MJSelectColor);
                sfs.PutInt("color", 0x20);
                return sfs;
            });
            HideDqBtns();
            TipText.ExCompHide();
        }

        /// <summary>
        /// 定缺：筒
        /// </summary>
        public void OnTongClick()
        {
            GameCenter.Network.OnRequestC2S((sfs) =>
            {
                sfs.PutInt(RequestKey.KeyType, NetworkProtocol.MJSelectColor);
                sfs.PutInt("color", 0x30);
                return sfs;
            });
            HideDqBtns();
            TipText.ExCompHide();
        }
    }
}