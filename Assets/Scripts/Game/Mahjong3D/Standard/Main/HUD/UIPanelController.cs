using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Game.Mahjong3D.Standard
{
    public class UIPanelController : MonoBehaviour
    {
        public Camera UICamera;
        public List<Transform> UIHierarchy;

        public void OnInit()
        {
            SubscriberEvent();
        }

        public void RefreshOtherPanelOnReconnected()
        {
            if (GameCenter.DataCenter.ConfigData.ScoreDouble)
            {
                GameCenter.Hud.GetPanel<PanelScoreDouble>().OnReconnectedUpdate();
            }
        }

        /// <summary>
        /// Op操作之前执行
        /// </summary>
        public void OnOperateUpdatePanel()
        {
            PanelOpreateMenu opPanel;
            if (GameCenter.Hud.TryGetPanel(out opPanel))
            {
                opPanel.HideButtons();
            }
            PanelChooseOperate choosePanel;
            if (GameCenter.Hud.TryGetPanel(out choosePanel))
            {
                choosePanel.Close();
            }
        }

        private void SubscriberEvent()
        {
            GameCenter.EventHandle.Subscriber((int)UIEventProtocol.ShowResult, OnShowResult);
            GameCenter.EventHandle.Subscriber((int)UIEventProtocol.QueryHuCard, OnQueryHuCard);
            GameCenter.EventHandle.Subscriber((int)UIEventProtocol.OnEventHandUp, OnEventHandUp);
            GameCenter.EventHandle.Subscriber((int)UIEventProtocol.ScoreDoubleCtrl, OnScoreDouble);
            GameCenter.EventHandle.Subscriber((int)UIEventProtocol.OperateMenuCtrl, OnOperateMenu);
            GameCenter.EventHandle.Subscriber((int)UIEventProtocol.OnShowHandUp, OnShowPanelHandUp);
            GameCenter.EventHandle.Subscriber((int)UIEventProtocol.ShowGameExplain, OnShowGameExplain);
            GameCenter.EventHandle.Subscriber((int)UIEventProtocol.ShowTotalResult, OnShowTotalResult);
            GameCenter.EventHandle.Subscriber((int)UIEventProtocol.ShowTitleMessage, ShowTitleMessage);
            GameCenter.EventHandle.Subscriber((int)UIEventProtocol.ShowChooseOperate, OnShowChooseOperate);
            GameCenter.EventHandle.Subscriber((int)UIEventProtocol.ShowMahFriendsPanel, OnShowMahFriendsPanel);
            GameCenter.EventHandle.Subscriber((int)UIEventProtocol.ShowChooseXjfd, OnShowChooseXjfdPanel);
            GameCenter.EventHandle.Subscriber((int)UIEventProtocol.ShowXjfdList, OnShowXjfdListPanel);
        }

        //显示游戏玩法
        private void OnShowGameExplain(EvtHandlerArgs args)
        {
            GameCenter.Hud.GetPanel<PanelGameExplain>().Open();
        }

        //查询胡牌
        private void OnQueryHuCard(EvtHandlerArgs args)
        {
            var data = args as QueryHuArgs;
            var panel = GameCenter.Hud.GetPanel<PanelQueryHuCard>();
            if (!data.PanelState)
            {
                panel.Close();
            }
            else
            {
                panel.Open(data);
            }
        }

        //提示框
        private void ShowTitleMessage(EvtHandlerArgs args)
        {
            GameCenter.Hud.GetPanel<PanelTitleMessage>().Open(args as ShowTitleMessageArgs);
        }

        //邀请麻友
        private void OnShowMahFriendsPanel(EvtHandlerArgs args)
        {
            GameCenter.Hud.GetPanel<PanelInviteFriends>().Open();
        }

        //下注（加漂）
        private void OnScoreDouble(EvtHandlerArgs args)
        {
            if (args == null)
            {
                GameCenter.Hud.GetPanel<PanelScoreDouble>().Open();
            }
            else
            {
                GameCenter.Hud.GetPanel<PanelScoreDouble>().Open(args as ScoreDoubleArgs);
            }
        }

        /// <summary>
        /// 选择菜单
        /// </summary>      
        private void OnShowChooseOperate(EvtHandlerArgs args)
        {
            var param = args as ChooseCgArgs;
            if (null == param) return;
            switch (param.Type)
            {
                case ChooseCgArgs.ChooseType.ChooseTing: GameCenter.Hud.GetPanel<PanelChooseOperate>().OnChooseTingCard(param); break;
                case ChooseCgArgs.ChooseType.ChooseCg: GameCenter.Hud.GetPanel<PanelChooseOperate>().Open(param); break;
            }
            //关闭按钮菜单界面
            GameCenter.Hud.ClosePanel<PanelOpreateMenu>();
        }

        //小结算界面
        private void OnShowResult(EvtHandlerArgs args)
        {
            if (GameUtils.CheckStopTask()) return;
            if (args != null)
            {
                GameCenter.Hud.GetPanel<PanelSingleResult>().Open(args as SingleResultArgs);
            }
        }

        //大结算界面
        private void OnShowTotalResult(EvtHandlerArgs args)
        {
            GameCenter.Hud.GetPanel<PanelTotalResult>().Do(p => p.Open());
        }

        //OperateMenu
        private void OnOperateMenu(EvtHandlerArgs args)
        {
            if (null == args)
            {
                GameCenter.Hud.GetPanel<PanelOpreateMenu>().Close();
            }
            else
            {
                GameCenter.Hud.GetPanel<PanelOpreateMenu>().Open(args as OpreateMenuArgs);
            }
        }

        //显示投票界面
        private void OnShowPanelHandUp(EvtHandlerArgs args)
        {
            GameCenter.Hud.OpenPanel<PanelHandup>();
        }

        //投票事件
        private void OnEventHandUp(EvtHandlerArgs args)
        {
            HandupEventArgs eventArgs = args as HandupEventArgs;
            PanelHandup panel = GameCenter.Hud.GetPanel<PanelHandup>();
            if (null != panel)
            {
                if (eventArgs.HandupType == DismissFeedBack.ApplyFor)
                {
                    panel.Open(eventArgs);
                }
                else
                {
                    panel.SetHandupState(eventArgs);
                }
            }
        }

        private void OnShowChooseXjfdPanel(EvtHandlerArgs args)
        {
            GameCenter.Hud.GetPanel<PanelChooseXjfd>().Open();
        }
        private void OnShowXjfdListPanel(EvtHandlerArgs args)
        {
            XjfdListArgs eventArgs = args as XjfdListArgs;
            PanelShowXjfdList panel = GameCenter.Hud.GetPanel<PanelShowXjfdList>();
            if (null != panel)
            {
                panel.Open(eventArgs);
            }
        }

        /// <summary>
        /// 播放特效
        /// </summary>    
        public void PlayUIEffect(PoolObjectType type)
        {
            Transform effectPos = UIHierarchy[(int)UIPanelhierarchy.EffectAndTip];
            PlayEffect(effectPos, type);
        }

        /// <summary>
        /// 根据玩家座位号播放特效
        /// </summary>
        /// <param name="chair">玩家游戏中座位号</param>
        /// <param name="type">特效类型 （注意：这其中会有许多不是特效的类型）</param>
        public void PlayPlayerUIEffect(int chair, PoolObjectType type)
        {
            Transform effectPos = GameCenter.Hud.GetPanel<PanelPlayersInfo>().PlayersOther.GetEffectPos(chair);
            PlayEffect(effectPos, type);
        }

        private void PlayEffect(Transform effectPos, PoolObjectType type)
        {
            effectPos.Do((o) =>
            {
                string name = type.ToString();
                EffectObject obj = GameCenter.Pools.GetPool<ObjectPoolComponent>(PoolUitlity.Po_EffectObject).Pop<EffectObject>(EffectObject.AssetsNamePrefix + name, (go) =>
                {
                    return go.Type == type;
                });
                if (obj != null)
                {
                    obj.ExSetParent(o);
                    obj.Execute();
                }
            });
        }

        public T SetPanel<T>(T panel, UIPanelhierarchy hierarchy) where T : UIPanelBase
        {
            if (null == panel) return null;
            RectTransform transform = UIHierarchy[(int)hierarchy] as RectTransform;
            panel.transform.ExSetParent(transform);
            panel.GetComponent<RectTransform>().sizeDelta = Vector2.zero;
            panel.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
            return panel;
        }
    }
}